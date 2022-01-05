using Awesomium.Windows.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Ink;

namespace IMOOC.EduCourses.PPTModule
{
    public sealed class PPTModel : NotificationObject, IDisposable
    {

        public List<StrokeCollection> strokesList;
        public List<List<int>> strokeIndexList;
        private int clickCount;
      //  private int clickCount1;
        private int?[] clickCountArray;
        private int currPPTSlide;
        public WebControl browser { set; get; }
        public int SlideCount { get; set; }
        public bool isNewOpen;
        private int currSlideIndex;
        /// <summary>
        /// 用于判断这个动作是不是当前页的第一个动作，用于区分第一个和第二个动作step都是1的问题
        /// </summary>
        private bool isStepBegin;

        private int currSlide;
        public int CurrSlide
        {
            get { return currSlide; }                      
        }

        private int currAdim;
        public int CurrAdim
        {
            get { return currAdim; }
        }


        public int CurrSlideIndex
        {
            get { return currSlideIndex; }
            set { RaisePropertyChanged(ref currSlideIndex, value, "CurrSlideIndex"); }
        }

        public PPTModel()
        {
         
        }

        public PPTModel(WebControl _borwser, int count)
        {            
            SlideCount = count;
            clickCountArray = new int?[SlideCount];
            strokesList = new List<StrokeCollection>();
            strokeIndexList = new List<List<int>>();

            for (int i = 0; i < SlideCount; i++)
            {
                strokesList.Add(new StrokeCollection());
                var list = new List<int>();
                list.Add(0);                
                strokeIndexList.Add(list);
            }

            browser = _borwser;
            //browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.LoadingFrameComplete += Browser_LoadingFrameComplete;

            currPPTSlide = 1;
            isStepBegin = false;
            isNewOpen = true;
            currAdim = 0;
            currSlide = 0;
        }

        public void JumpSlideAndAnim()
        {
            browser.ExecuteJavascript("Presentation.JumpToAnim("+ currAdim+","+ currSlide+");");
        }

        private void Browser_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            try
            {
                clickCount = (int)browser.ExecuteJavascriptWithResult("Presentation.ClickCount();");
                clickCountArray[CurrSlideIndex] = clickCount;
            }
            catch (Exception)
            {
                return;
            }
            
        }

        public void Next(ref int slide,ref int anim,ref int clickCount,ref bool isNextSlide)
        {
            if (isStepBegin==true)
            {
                anim = 0;
            }
            else
            {
                anim = (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().step");
            }
            if (anim== (int)browser.ExecuteJavascriptWithResult("Presentation.ClickCount();"))
            {
                anim = -1;
            }
            clickCount = (int)browser.ExecuteJavascriptWithResult("Presentation.ClickCount();");
            browser.ExecuteJavascript("Presentation.Next();");            
            if (currPPTSlide != (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().slide"))
            {
                currPPTSlide = (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().slide");                
                isNextSlide = true;
                slide = currPPTSlide;
                CurrSlideIndex++;
                isStepBegin = true;             
            }     
            else
            {
                isNextSlide = false;
                slide = -1;
                isStepBegin = false;
            }

            currSlide = CurrSlideIndex+1;
            currAdim= (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().step");

        }

        public void Previous(ref int slide, ref int anim, ref bool isPrevSlide)
        {
            anim = (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().step");
            browser.ExecuteJavascript("Presentation.Prev();");           
            if (currPPTSlide != (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().slide"))
            {
                currPPTSlide = (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().slide");
                isPrevSlide = true;
                slide = currPPTSlide;
                CurrSlideIndex--;
            } 
            else
            {
                isPrevSlide = false;
                slide = -1;
            }

            currSlide = CurrSlideIndex+1;
            currAdim = (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().step");
        }

        public Utils.PPTStatus CurrPPTStatus() 
        {
            var currStatus = new Utils.PPTStatus();
            currStatus.Slide= (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().slide");
            currStatus.Anim= (int)browser.ExecuteJavascriptWithResult("Presentation.CurrentStatus().step");
            return currStatus;
        }

        public StrokeCollection GetCurrStrokes()
        {
            return strokesList[CurrSlideIndex];
        }

        public List<int> GetCurrPageHistoryMaxCount()
        {
            return strokeIndexList[CurrSlideIndex];
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    strokesList = null;
                    strokeIndexList = null;
                    clickCountArray = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                if (browser!=null)
                {
                    try
                    {
                        browser.Dispose();
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                }
                
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~PPTModel()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion



    }



}
