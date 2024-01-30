/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf.Tagutils;
using iText.Test;

namespace iText.Pdfocr.Structuretree {
    [NUnit.Framework.Category("UnitTest")]
    public class LogicalStructureTreeItemTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddChildTest() {
            LogicalStructureTreeItem parent = new LogicalStructureTreeItem();
            LogicalStructureTreeItem child1 = new LogicalStructureTreeItem();
            LogicalStructureTreeItem child2 = new LogicalStructureTreeItem();
            child1.AddChild(child2);
            parent.AddChild(child1);
            parent.AddChild(child2);
            NUnit.Framework.Assert.AreEqual(2, parent.GetChildren().Count);
            NUnit.Framework.Assert.AreEqual(0, child1.GetChildren().Count);
            NUnit.Framework.Assert.AreEqual(parent, child1.GetParent());
            NUnit.Framework.Assert.AreEqual(parent, child2.GetParent());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveChildTest() {
            LogicalStructureTreeItem parent = new LogicalStructureTreeItem();
            LogicalStructureTreeItem child1 = new LogicalStructureTreeItem();
            LogicalStructureTreeItem child2 = new LogicalStructureTreeItem();
            child1.AddChild(child2);
            parent.AddChild(child1);
            parent.AddChild(child2);
            NUnit.Framework.Assert.IsTrue(parent.RemoveChild(child1));
            NUnit.Framework.Assert.IsFalse(parent.RemoveChild(child1));
            NUnit.Framework.Assert.AreEqual(1, parent.GetChildren().Count);
        }

        [NUnit.Framework.Test]
        public virtual void AccessibilityPropertiesTest() {
            LogicalStructureTreeItem item = new LogicalStructureTreeItem().SetAccessibilityProperties(new DefaultAccessibilityProperties
                ("Some role"));
            NUnit.Framework.Assert.AreEqual("Some role", item.GetAccessibilityProperties().GetRole());
        }
    }
}
