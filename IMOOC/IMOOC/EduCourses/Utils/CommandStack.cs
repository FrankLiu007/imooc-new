using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Ink;
using IMOOC.EduCourses.Utils;

namespace IMOOC.EduCourses
{
    public class CommandStack
    {
        private InkCanvasPage _page;
        private Stack<CommandItem> _undoStack;
        private Stack<CommandItem> _redoStack;

        private bool _disableChangeTracking; // reentrancy guard: disables tracking of programmatic changes 
        // (eg, in response to undo/redo ops)

        public CommandStack(InkCanvasPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            _page = page;
            _undoStack = new Stack<CommandItem>();
            _redoStack = new Stack<CommandItem>();
            _disableChangeTracking = false;
        }
        
        public InkCanvasPage Page
        {
            get { return _page; }
        }


        /// <summary>
        /// Only undo if there are more items in the stack to step back into.
        /// </summary>
        public bool CanUndo
        {
            get { return (_undoStack.Count > 0); }
        }

        /// <summary>
        /// Only undo if one or more steps back in the stack.
        /// </summary>
        public bool CanRedo
        {
            get { return (_redoStack.Count > 0); }
        }

        /// <summary>
        /// Add an item to the top of the command stack
        /// </summary>
        public IndicaterDragCompletedEventArgs Undo()
        {
            if (!CanUndo) throw new InvalidOperationException("No actions to undo");

            CommandItem item = _undoStack.Pop();
            IndicaterDragCompletedEventArgs args;
            // Invoke the undo operation, with change-tracking temporarily suspended.
            _disableChangeTracking = true;
            try
            {
                args = item.Undo();
            }
            finally
            {
                _disableChangeTracking = false;
            }

            //place this item on the redo stack
            _redoStack.Push(item);
            return args;
        }

        /// <summary>
        /// Take the top item off the command stack.
        /// </summary>
        public IndicaterDragCompletedEventArgs Redo()
        {
            if (!CanRedo) throw new InvalidOperationException();

            CommandItem item = _redoStack.Pop();
            IndicaterDragCompletedEventArgs args;
            // Invoke the redo operation, with change-tracking temporarily suspended.
            _disableChangeTracking = true;
            try
            {
                args = item.Redo();
            }
            finally
            {
                _disableChangeTracking = false;
            }

            //place this item on the undo stack
            _undoStack.Push(item);
            return args;
        }

        /// <summary>
        /// Add a command item to the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(CommandItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            // Ensure we don't enqueue new items if we're being changed programmatically.
            if (_disableChangeTracking)
            {
                return;
            }

            _undoStack.Push(item);

            //clear the redo stack
            if (_redoStack.Count > 0)
            {
                _redoStack.Clear();
            }
        }
        
        
    }

    /// <summary>
    /// Derive from this class for every undoable/redoable operation you wish to support.
    /// </summary>
    public abstract class CommandItem
    {
        // Interface
        public abstract IndicaterDragCompletedEventArgs Undo();
        public abstract IndicaterDragCompletedEventArgs Redo();

        // Allows multiple subsequent commands of the same type to roll-up into one 
        // logical undoable/redoable command -- return false if newitem is incompatable.
        //public abstract bool Merge(CommandItem newitem);

        // Implementation
        protected CommandStack _commandStack;

        protected CommandItem(CommandStack commandStack)
        {
            _commandStack = commandStack;
        }
    }

    /// <summary>
    /// This operation covers collecting new strokes, stroke-erase, and point-erase.
    /// </summary>
    public class StrokesAddedOrRemovedCI : CommandItem
    {
        private StrokeCollection _added, _removed;

        public StrokesAddedOrRemovedCI(CommandStack commandStack,
            StrokeCollection added, StrokeCollection removed) : base(commandStack)
        {
            _added = added;
            _removed = removed;
        }

        public override IndicaterDragCompletedEventArgs Undo()
        {
            try
            {
                _commandStack.Page.Strokes.Remove(_added);
                _commandStack.Page.Strokes.Add(_removed);
                
            }
            catch (Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "撤销的笔画可能存在问题！");
                message.ShowDialog();
                Public.Log.WriterLog("撤销的笔画可能存在问题！" +ex.Message);
            }
            return null;

        }

        public override IndicaterDragCompletedEventArgs Redo()
        {
            try
            {
                _commandStack.Page.Strokes.Add(_added);
                _commandStack.Page.Strokes.Remove(_removed);
            }
            catch (Exception ex)
            {
                MessageWin message = new MessageWin(MessageWinType.Prompt, "重做的笔画可能存在问题！");
                message.ShowDialog();
                Public.Log.WriterLog("重做的笔画可能存在问题！"+ex.Message);
            }
            
            return null;
        }

    }

    /// <summary>
    /// This operation covers move and resize operations.
    /// </summary>
    public class SelectionMovedOrResizedCI : CommandItem
    {
        private StrokeCollection _selectedStrokes;
        private InkCanvasChildCollection _selectedAllChild;
        Rect _newrect, _oldrect;

        public SelectionMovedOrResizedCI(CommandStack commandStack, StrokeCollection selectedStrokes,
            InkCanvasChildCollection selectedChild, Rect newrect, Rect oldrect) : base(commandStack)
        {
            _selectedStrokes = selectedStrokes;
            _selectedAllChild = selectedChild;
            _newrect = newrect;
            _oldrect = oldrect;
        }

        public override IndicaterDragCompletedEventArgs Undo()
        {
            Matrix m = GetTransformFromRectToRect(_newrect, _oldrect);
            _selectedStrokes.Transform(m, false);
            _selectedAllChild.Transform(_newrect, _oldrect);
            return new EduCourses.IndicaterDragCompletedEventArgs(_newrect, _oldrect, _selectedStrokes, _selectedAllChild);
        }

        public override IndicaterDragCompletedEventArgs Redo()
        {
            Matrix m = GetTransformFromRectToRect(_oldrect, _newrect);
            _selectedStrokes.Transform(m, false);
            _selectedAllChild.Transform(_oldrect, _newrect);
            return new EduCourses.IndicaterDragCompletedEventArgs(_oldrect, _newrect, _selectedStrokes, _selectedAllChild);
        }

        public static Matrix GetTransformFromRectToRect(Rect src, Rect dst)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-src.X, -src.Y);
            m.Scale(dst.Width / src.Width, dst.Height / src.Height);
            m.Translate(+dst.X, +dst.Y);
            return m;
        }
    }

    public class AllChildAddedOrRemovedCI : CommandItem
    {
        private InkCanvasChildCollection _added, _removed;

        public AllChildAddedOrRemovedCI(CommandStack commandStack,
            InkCanvasChildCollection added, InkCanvasChildCollection removed) : base(commandStack)
        {
            _added = added;
            _removed = removed;
        }

        public override IndicaterDragCompletedEventArgs Undo()
        {
            _commandStack.Page.AllChild.Add(_removed);
            _commandStack.Page.AllChild.Remove(_added);
            return null;
        }

        public override IndicaterDragCompletedEventArgs Redo()
        {
            _commandStack.Page.AllChild.Add(_added);
            _commandStack.Page.AllChild.Remove(_removed);
            return null;
        }
    }
}
