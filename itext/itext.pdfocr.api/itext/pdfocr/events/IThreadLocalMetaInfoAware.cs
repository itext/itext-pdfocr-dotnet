/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Counter.Event;

namespace iText.Pdfocr.Events {
    /// <summary>
    /// The interface which holds a thread local meta info,
    /// meaning different threads operate with independent and different meta infos.
    /// </summary>
    public interface IThreadLocalMetaInfoAware {
        /// <summary>Gets the meta info which is held by the interface.</summary>
        /// <returns>the held thread local meta info</returns>
        IMetaInfo GetThreadLocalMetaInfo();

        /// <summary>Sets a thread local meta info.</summary>
        /// <param name="metaInfo">a thread local meta info to be held</param>
        /// <returns>
        /// this
        /// <see cref="IThreadLocalMetaInfoAware"/>
        /// </returns>
        IThreadLocalMetaInfoAware SetThreadLocalMetaInfo(IMetaInfo metaInfo);
    }
}
