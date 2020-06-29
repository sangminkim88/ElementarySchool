namespace GAIA2020.Design
{
    using HmDraw.Entities;
    using LogStyle;

    #region Interfaces

    public interface IDrillLog
    {
        #region Methods

        void iEditorEnter(LogTag logTag, object value, bool isTab);

        void iEditorHide();

        void iEditorShow(LogTag logTag, string value, double lx, double ly, double w, double h);

        void iMouseDown(double mx, double my, double px, double py, double pz);

        void iMouseUp(double mx, double my, double px, double py, double pz);

        void iSelectEntity(string stylyr, HmEntity enty);

        #endregion
    }

    public interface ILoadComplete
    {
        void iLoadComplete();
    }

    public interface ILogStyleChanged
    {
        void iLogStyleChange();
    }
    #endregion
}
