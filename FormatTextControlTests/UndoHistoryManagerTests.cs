using FormatTextControl;
using System.ComponentModel.DataAnnotations;

namespace FormatTextControlTests
{
    [TestClass]
    public class UndoHistoryManagerTests
    {
        [TestMethod]
        public void TestUndoOne()
        {
            var manager = new UndoHistoryManager();
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 0), new TextPos(0, 1), "a", true));
            var ur = manager.GetNextUndo();

            Assert.IsNotNull(ur);
            Assert.AreEqual("a", ur.Text);
        }

        [TestMethod]
        public void TestUndoTre()
        {
            var manager = new UndoHistoryManager();
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 0), new TextPos(0, 1), "a", true));
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 1), new TextPos(0, 4), "bcd", true));
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 4), new TextPos(0, 5), "x", true));
            var ur = manager.GetNextUndo();

            Assert.IsNotNull(ur);
            Assert.AreEqual("x", ur.Text);

            ur = manager.GetNextUndo();

            Assert.IsNotNull(ur);
            Assert.AreEqual("bcd", ur.Text);

            ur = manager.GetNextUndo();

            Assert.IsNotNull(ur);
            Assert.AreEqual("a", ur.Text);

            ur = manager.GetNextUndo();
            Assert.IsNull(ur);
        }

        [TestMethod]
        public void TestUndoThenRedo()
        {
            var manager = new UndoHistoryManager();
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 0), new TextPos(0, 1), "a", true));
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 1), new TextPos(0, 4), "bcd", true));
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 4), new TextPos(0, 5), "x", true));
            var ur = manager.GetNextUndo();
            Assert.IsNotNull(ur);
            Assert.AreEqual("x", ur.Text);

            ur = manager.GetNextRedo();
            Assert.IsNotNull(ur);
            Assert.AreEqual("x", ur.Text);

            ur = manager.GetNextRedo();
            Assert.IsNull(ur);
        }

        [TestMethod]
        public void TestUndoThenAdd()
        {
            var manager = new UndoHistoryManager();
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 0), new TextPos(0, 1), "a", true));
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 1), new TextPos(0, 4), "bcd", true));
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 4), new TextPos(0, 5), "x", true));
            manager.GetNextUndo();
            manager.GetNextUndo();
            manager.AddUndoRecord(new UndoRecord(new TextPos(0, 1), new TextPos(0, 4), "yyy", true));
            var ur = manager.GetNextUndo();
            Assert.IsNotNull(ur);
            Assert.AreEqual("yyy", ur.Text);

            ur = manager.GetNextUndo();
            Assert.IsNotNull(ur);
            Assert.AreEqual("a", ur.Text);
        }
    }
}