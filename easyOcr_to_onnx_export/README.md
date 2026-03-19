# easyOcr_to_onnx_export script

### Disclaimer

There is no official method for converting EasyOCR models to ONNX, so a custom approach was required.  
The converted recognition models retain the same inputs and outputs as the original versions, while the detection models were slightly adjusted to better fit our use case.
<br>


## Setup Instructions

Follow these steps to set up a virtual environment and install the required dependencies.

### 1. Create a virtual environment

```bash
python -m venv .venv
```

---

### 2. Activate the virtual environment

* **Linux / macOS:**

```bash
source .venv/bin/activate
```

* **Windows:**

```bash
.venv\Scripts\activate
```

---

### 3. Install dependencies

Install all required packages using the `requirements.txt` file:

```bash
pip install -r requirements.txt
```

---

### 4. Run the script

```bash
python easyOcr_to_onnx_export.py <model_dir>
```

Replace `<model_dir>` with the path to your EasyOCR model directory.

