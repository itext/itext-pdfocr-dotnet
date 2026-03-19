#!/usr/bin/env python3
import argparse
import os.path

import easyocr
from easyocr import config
from easyocr.craft import CRAFT
from easyocr.detection import copyStateDict

import torch


detection_models = (
    'craft',
)
recognition_models_gen1 = (
    'arabic_g1',
    'bengali_g1',
    'cyrillic_g1',
    'devanagari_g1',
    'japanese_g1',
    'korean_g1',
    'latin_g1',
    # FIXME: this one causes issues during export
    # 'tamil_g1',
    'thai_g1',
    'zh_sim_g1',
    'zh_tra_g1',
)
recognition_models_gen2 = (
    'cyrillic_g2',
    'english_g2',
    'japanese_g2',
    'kannada_g2',
    'korean_g2',
    'latin_g2',
    'telugu_g2',
    'zh_sim_g2',
)
recognition_models = recognition_models_gen1 + recognition_models_gen2


# Detection model
class TrimmedCRAFT(CRAFT):
    def forward(self, x):
        # Ignoring "feature"
        y, _ = super().forward(x)
        # Transposing result back to BCHW
        return y.permute(0, 3, 1, 2)


def get_detector(trained_model, device='cpu'):
    net = TrimmedCRAFT()
    net.load_state_dict(copyStateDict(torch.load(trained_model, map_location=device, weights_only=False)))
    torch.quantization.quantize_dynamic(net, dtype=torch.qint8, inplace=True)
    net.eval()
    return net


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('model_dir', help='directory with EasyOCR models')
    model_dir = parser.parse_args().model_dir

    for recognition_model in recognition_models:
        print(f'Exporting {recognition_model}...')
        gen = 'gen1' if recognition_model.endswith('_g1') else 'gen2'
        filename: str = config.recognition_models[gen][recognition_model]['filename']
        reader = easyocr.Reader(
            lang_list=['en'],
            gpu=False,
            model_storage_directory=model_dir,
            recog_network=recognition_model,
            quantize=False,
        )
        # AdaptiveAvgPool2d cannot be exported to ONNX
        # Specifying a static one instead assuming imgH=64
        reader.recognizer.AdaptiveAvgPool = torch.nn.AvgPool2d((1, 3))
        dummy_input = (
            torch.randn(1, 1, 64, 512),
            torch.randn(1, 512),
        )
        torch.onnx.export(
            reader.recognizer,
            dummy_input,
            os.path.join(model_dir, filename.rsplit('.', 1)[0] + '.onnx'),
            export_params=True,
            input_names=('input', 'text',),
            output_names=('preds',),
            dynamic_axes={
                "input": {0: 'batch_size', 3: 'width'},
                "text": {0: 'batch_size', 1: 'batch_max_length'},
            },
        )

    print('Exporting CRAFT...')
    filename: str = config.detection_models['craft']['filename']
    dummy_input = (torch.randn(1, 3, 2560, 2560),)
    model = get_detector(os.path.join(model_dir, filename))
    torch.onnx.export(
        model,
        dummy_input,
        os.path.join(model_dir, filename.rsplit('.', 1)[0] + '.onnx'),
        export_params=True,
        input_names=('images',),
        output_names=('y',),
        dynamic_axes={
            "images": {0: 'batch_size', 2: 'height', 3: 'width'},
        },
    )

if __name__ == '__main__':
    main()
