using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace IMOOC.EduCourses.Cad
{
    public class ItemStack
    {
        private Stack<ActionItem> _undoStack;
        private Stack<ActionItem> _redoStack;
        private Collection<BaseShape> shapes;
        public Collection<BaseShape> Elements { get { return shapes; } }

        private bool _disableChangeTracking;

        public ItemStack(Collection<BaseShape> shapeList)
        {
            if (shapeList == null)
            {
                throw new ArgumentNullException("shapeList");
            }

            shapes = shapeList;
            _undoStack = new Stack<ActionItem>();
            _redoStack = new Stack<ActionItem>();
            _disableChangeTracking = false;
        }


        public bool CanUndo
        {
            get { return (_undoStack.Count > 0); }
        }

        public bool CanRedo
        {
            get { return (_redoStack.Count > 0); }
        }

        public void Undo()
        {
            if (!CanUndo) throw new InvalidOperationException("No actions to undo");

            ActionItem item = _undoStack.Pop();

            _disableChangeTracking = true;
            try
            {
                item.Undo();
            }
            finally
            {
                _disableChangeTracking = false;
            }

            _redoStack.Push(item);
        }

        public void Redo()
        {
            if (!CanRedo) throw new InvalidOperationException();

            ActionItem item = _redoStack.Pop();

            _disableChangeTracking = true;
            try
            {
                item.Redo();
            }
            finally
            {
                _disableChangeTracking = false;
            }

            _undoStack.Push(item);
        }

        public void Enqueue(ActionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (_disableChangeTracking)
            {
                return;
            }

            _undoStack.Push(item);

            if (_redoStack.Count > 0)
            {
                _redoStack.Clear();
            }
        }
    }

    public abstract class ActionItem
    {
        public abstract void Undo();
        public abstract void Redo();

        protected ItemStack _itemStack;

        protected ActionItem(ItemStack itemtack)
        {
            _itemStack = itemtack;
        }
    }

    public class ShapeAddedAI : ActionItem
    {
        private BaseShape _added;

        public ShapeAddedAI(ItemStack itemStack, BaseShape added) : base(itemStack)
        {
            _added = added;
        }

        public override void Undo()
        {
            _itemStack.Elements.Remove(_added);
        }

        public override void Redo()
        {
            _itemStack.Elements.Add(_added);
        }

    }

    public class ShapeRemovedAI : ActionItem
    {
        private BaseShape _removed;

        public ShapeRemovedAI(ItemStack itemtack, BaseShape removed) : base(itemtack)
        {
            _removed = removed;
        }

        public override void Redo()
        {
            _itemStack.Elements.Remove(_removed);
        }

        public override void Undo()
        {
            _itemStack.Elements.Add(_removed);
        }
    }
}
