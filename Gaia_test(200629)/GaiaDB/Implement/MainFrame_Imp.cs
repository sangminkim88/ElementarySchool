using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaiaDB
{
    public interface IMainFrame
    {
        /// <summary>모델저장시의 썸네일 이미지를 만들어 넘겨줍니다.</summary>
        System.Drawing.Bitmap Get_PreViewImage();
    }
    public class MainFrame_Imp
    {
        private static object m_ImpleMainFrame = null;

        public static void Set_MainFrame(IMainFrame mainFrame)
        { m_ImpleMainFrame = mainFrame; }

        public static IMainFrame Get_MainFrame()
        { return (IMainFrame)m_ImpleMainFrame; }
    }
}
