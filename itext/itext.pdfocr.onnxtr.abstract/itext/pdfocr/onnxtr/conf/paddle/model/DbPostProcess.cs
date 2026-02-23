/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Commons.Utils;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Conf.Paddle.Model {
    /// <summary>
    /// POJO for the DBPostProcess post-processor object under a
    /// <c>PostProcess</c>
    /// key in a config file.
    /// </summary>
    public class DbPostProcess : PostProcess {
        /// <summary>Expected name for the CTCLabelDecode post-processor.</summary>
        public const String NAME = "DBPostProcess";

        private readonly float thresh;

        private readonly float boxThresh;

        private readonly float unclipRatio;

        private readonly int maxCandidates;

        private readonly bool useDilation;

        private readonly ScoreMode scoreMode;

        private readonly BoxType boxType;

        /// <summary>Creates a new POJO for the config file object.</summary>
        /// <param name="thresh">
        /// value under the
        /// <paramref name="thresh"/>
        /// key
        /// </param>
        /// <param name="boxThresh">
        /// value under the
        /// <c>box_thresh</c>
        /// key
        /// </param>
        /// <param name="unclipRatio">
        /// value under the
        /// <c>unclip_ratio</c>
        /// key
        /// </param>
        /// <param name="maxCandidates">
        /// value under the
        /// <c>max_candidates</c>
        /// key
        /// </param>
        /// <param name="useDilation">
        /// value under the
        /// <c>use_dilation</c>
        /// key
        /// </param>
        /// <param name="scoreMode">
        /// value under the
        /// <c>score_mode</c>
        /// key
        /// </param>
        /// <param name="boxType">
        /// value under the
        /// <c>box_type</c>
        /// key
        /// </param>
        public DbPostProcess(float thresh, float boxThresh, float unclipRatio, int maxCandidates, bool useDilation
            , ScoreMode scoreMode, BoxType boxType) {
            this.thresh = thresh;
            this.boxThresh = boxThresh;
            this.unclipRatio = unclipRatio;
            this.maxCandidates = maxCandidates;
            this.useDilation = useDilation;
            this.scoreMode = Objects.RequireNonNull(scoreMode);
            this.boxType = Objects.RequireNonNull(boxType);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetName() {
            return NAME;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>thresh</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>thresh</c>
        /// key
        /// </returns>
        public virtual float GetThresh() {
            return thresh;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>box_thresh</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>box_thresh</c>
        /// key
        /// </returns>
        public virtual float GetBoxThresh() {
            return boxThresh;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>unclip_ratio</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>unclip_ratio</c>
        /// key
        /// </returns>
        public virtual float GetUnclipRatio() {
            return unclipRatio;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>max_candidates</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>max_candidates</c>
        /// key
        /// </returns>
        public virtual int GetMaxCandidates() {
            return maxCandidates;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>use_dilation</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>use_dilation</c>
        /// key
        /// </returns>
        public virtual bool GetUseDilation() {
            return useDilation;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>score_mode</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>score_mode</c>
        /// key
        /// </returns>
        public virtual ScoreMode GetScoreMode() {
            return scoreMode;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>box_type</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>box_type</c>
        /// key
        /// </returns>
        public virtual BoxType GetBoxType() {
            return boxType;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DbPostProcess that = (iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DbPostProcess
                )o;
            return JavaUtil.FloatCompare(thresh, that.thresh) == 0 && JavaUtil.FloatCompare(boxThresh, that.boxThresh)
                 == 0 && JavaUtil.FloatCompare(unclipRatio, that.unclipRatio) == 0 && maxCandidates == that.maxCandidates
                 && useDilation == that.useDilation && scoreMode == that.scoreMode && boxType == that.boxType;
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)thresh, boxThresh, unclipRatio, maxCandidates, useDilation, scoreMode
                , boxType);
        }
    }
}
