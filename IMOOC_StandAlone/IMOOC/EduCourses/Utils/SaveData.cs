using System;
using System.Collections.Generic;
using System.Windows;

namespace IMOOC.EduCourses.Utils
{
    public sealed class SaveData
    {
        public WindowSize size { get; set; }
        public Background background { get; set; }
        public AudioTimeLength audioTimeLeng { get; set; }
        public List<Page> pages;
        public List<PPT> PPTS { get; set; }
        public List<Picture> pictures { get; set; }
        public List<Actions> actions;
        public List<OldPage> oldPages { get; set; }
        public static readonly int decimalPoint = 3;

        public SaveData()
        {
            size = new WindowSize();
            background = new Background();
            audioTimeLeng = new AudioTimeLength();
            pages = new List<Page>();
            PPTS = new List<Utils.PPT>();
            pictures = new List<Picture>();
            actions = new List<Actions>();
            oldPages = new List<OldPage>();

        }
    }

    public sealed class AudioTimeLength
    {
        //public string audioNmae { get; set; }
        public double timeLength { get; set; }
    }

    public sealed class WindowSize
    {        
        private double width;
        private double height;

        public double w
        {
            get { return width; }
            set { width = Math.Round(value, SaveData.decimalPoint); }
        }
        public double h
        {
            get { return height; }
            set { height = Math.Round(value, SaveData.decimalPoint); }
        }
    }

    public sealed class Background
    {
        public string type;
        public GradientCircle start;
        public GradientCircle end;
        public GradientColor[] colors;
    }

    public sealed class GradientCircle
    {
        private double x1;    
        public double x
        {
            get { return x1; }
            set { x1 = Math.Round(value, SaveData.decimalPoint); }
        }

        private double y1;
        public double y
        {
            get { return y1; }
            set { y1 = Math.Round(value, SaveData.decimalPoint); }
        }

        public double radiusX { get; set; }
        public double radiusY { get; set; }
    }

    public sealed class GradientColor
    {
        public double pos { get; set; }
        public string value { get; set; }
    }
    
    public sealed class Page
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public sealed class PPT
    {
        public string name { get; set; }
        public string src { get; set; }
    }

    public class Actions
    {
        public double time { get; set; }
        public string type { get; set; }
    }

    public sealed class StrokeAction : Actions
    {
        //public StrokeX stroke { get; set; }
        public double duration { get; set; }
        public int index { get; set; }
        public Option options { get; set; }
        public List<Dot> dots { get; set; }
    }

    public sealed class UpdataIndicater: Actions
    {
        private double x1;
        public double x
        {
            get { return x1; }
            set { x1 = Math.Round(value, SaveData.decimalPoint); }
        }

        private double y1;
        public double y
        {
            get { return y1; }
            set { y1 = Math.Round(value, SaveData.decimalPoint); }
        }

        public double duration { get; set; }

        private double w;        
        public double width
        {
            get { return w; }
            set { w = Math.Round(value, SaveData.decimalPoint); }
        }

        private double h;
        public double height
        {
            get { return h; }
            set { h = Math.Round(value, SaveData.decimalPoint); }
        }
    }

    public sealed class Option
    {        
        public string color { get; set; }
        public string dotType { get; set; }
    }


    public sealed class Dot
    {
        private double x1;
        public double x
        {
            get { return x1; }
            set { x1 = Math.Round(value, SaveData.decimalPoint); }
        }

        private double y1;
        public double y
        {
            get { return y1; }
            set { y1 = Math.Round(value, SaveData.decimalPoint); }
        }
        public double radius { get; set; }
    }

    public sealed class ChoosePageAction : Actions
    {
        public string index { get; set; }
    }

    public sealed class InsertPictureAction : Actions
    {
        public string id { get; set; }
        public int index { get; set; }
        public Pos pos { get; set; }

        private double w;        
        public double width
        {
            get { return w; }
            set { w = Math.Round(value, SaveData.decimalPoint); }
        }

        private double h;
        public double height
        {
            get { return h; }
            set { h = Math.Round(value, SaveData.decimalPoint); }
        }
    }

    public sealed class Pos
    {
        private double x1;
        public double x
        {
            get { return x1; }
            set { x1 = Math.Round(value, SaveData.decimalPoint); }
        }

        private double y1;
        public double y
        {
            get { return y1; }
            set { y1 = Math.Round(value, SaveData.decimalPoint); }
        }

        public Pos(double PosX,double PosY)
        {
            x = PosX;
            y = PosY;
        }
    }

    public sealed class MoveAction : Actions
    {
        public MoveTransformContent[] contents { get; set; }
    }

    public sealed class ScaleAction :Actions
    {
        public ScaleTransfromContent[] contents { get; set; }
    }

    public class TransformContent
    {
        public int index { get; set; }
        public string type { get; set; }
    }

    public sealed class MoveTransformContent :TransformContent
    {
        public Pos pos { get; set; }
    }

    public sealed class ScaleTransfromContent : TransformContent
    {
        public double scale { get; set; }
        public Pos pos { get; set; }
    }

    public sealed class DeleteAction :Actions
    {
        public DeleteContent[] contents { get; set; }
    }

    public sealed class ChoosePPTAction:Actions
    {
        public string name { get; set; }
        public string page { get; set; } 
    }

    public sealed class PPTActionAction : Actions
    {
        public int slide { get; set; }
        public int anim { get; set; }
        public string page { get; set; } 
    }

    public sealed class PptGotoAction :Actions
    {
        public int page { get; set; }
        public int step { get; set; }
    }

    public class OldPage
    {
        public string index { get; set; }
        public Content[] contents { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
    }

    public sealed class DeleteContent :Content
    {
        public string index { get; set; }
    }
    
    
    public sealed class LineContent : Content
    {
        public Dot[] dots { get; set; }
        public int index { get; set; }
        public Option options { get; set; }
    }
    
    public sealed class PictrueContent:Content
    {
        public string id { get; set; }
        public int index { get; set; }
        public Pos pos { get; set; }

        private double w;
        public double width
        {
            get { return w; }
            set { w = Math.Round(value, SaveData.decimalPoint); }
        }

        private double h;
        public double height
        {
            get { return h; }
            set { h = Math.Round(value, SaveData.decimalPoint); }
        }
    }

    public sealed class Picture
    {
        public List<string> id { get; set; }
        public string src { get; set; }

        public Picture()
        {
            id = new List<string>();
        }
    }
}
