using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WpfBase.Bases
{
    /// <summary>
    /// PropertyChanged Delegate
    /// SetValue 사용 시 전달 된 Action 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public delegate void RaisePropertyChangedDelegate<T>(T oldValue, T newValue);
    public delegate void RaisePropertyChangedDelegateWithSender<T>(object sender, T oldValue, T newValue);

    public abstract class NotifyPropertyBase : INotifyPropertyChanged, IDisposable, ISupportInitialize
    {
        #region  INotifyPropertyChanged
        /// <summary>        
        ///    속성 값이 변경될 때 발생합니다.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region  IDisposable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        { }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region  ISupportInitialize
        public virtual void BeginInit() { }
        public virtual void EndInit() { }
        #endregion

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region SetValue
        /// <summary>
        /// The ThreadBarrier's captured SynchronizationContext
        /// </summary>
        private readonly SynchronizationContext _syncContext = AsyncOperationManager.SynchronizationContext;

        /// <summary>
        /// 속성값 Binding 설정
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        protected void SetValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        {
            SetValue<T>(ref field, newValue, false, null, propertyName);
        }
        /// <summary>
        /// 속성값 Binding 설정(PropertyChanged Delegate 전달)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="raiseChangedDelegate">PropertyChanged Delegate</param>
        /// <param name="propertyName"></param>
        protected void SetValue<T>(ref T field, T newValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate, [CallerMemberName] string propertyName = "")
        {
            SetValue<T>(ref field, newValue, false, raiseChangedDelegate, propertyName);
        }
        /// <summary>
        /// 속성값 Binding 설정(동일값 PropertyChanged 발생유무)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="disposeOldValue">oldValue Dispose 여부</param>
        /// <param name="isEquals">oldValue = newValue 동일한 값 PropertyChanged 이벤트 발생 여부</param>
        /// <param name="raiseChangedDelegate">PropertyChanged Delegate</param>
        /// <param name="propertyName"></param>
        protected void SetValue<T>(ref T field, T newValue, bool disposeOldValue, bool isEquals, RaisePropertyChangedDelegate<T> raiseChangedDelegate = null, [CallerMemberName] string propertyName = "")
        {
            SetValue<T>(ref field, newValue, disposeOldValue, raiseChangedDelegate, isEquals, propertyName);
        }

        /// <summary>
        /// 현재값과 발생값이 동일한 경우 Changed Event 가 발생하지 않는다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="disposeOldValue">oldValue Dispose 여부</param>
        /// <param name="raiseChangedDelegate">PropertyChanged Delegate</param>
        /// <param name="propertyName"></param>
        private void SetValue<T>(ref T field, T newValue, bool disposeOldValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, newValue)) return;
            T oldValue = field;
            field = newValue;

            if (_syncContext == null)
                PostCallback<T>(newValue, disposeOldValue, raiseChangedDelegate, oldValue, propertyName);
            else
            {
                _syncContext.Send(delegate {
                    PostCallback<T>(newValue, disposeOldValue, raiseChangedDelegate, oldValue, propertyName);
                }, null);
            }
        }

        /// <summary>
        /// 현재값과 들어온 값이 같은 경우에도 이벤트를 발생하게 한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="disposeOldValue"></param>
        /// <param name="raiseChangedDelegate"></param>
        /// <param name="isEquals">OldValue.Equals(NewValue) = true 일 경우에도 raiseProperty 이벤트를 발생 시킨다.</param>
        /// <param name="propertyName"></param>
        private void SetValue<T>(ref T field, T newValue, bool disposeOldValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate, bool isEquals, [CallerMemberName] string propertyName = "")
        {
            if (!isEquals && Equals(field, newValue)) return;
            T oldValue = field;
            field = newValue;

            if (_syncContext == null)
                PostCallback<T>(newValue, disposeOldValue, raiseChangedDelegate, oldValue, propertyName);
            else
            {
                _syncContext.Send(delegate {
                    PostCallback<T>(newValue, disposeOldValue, raiseChangedDelegate, oldValue, propertyName);
                }, null);
            }
        }

        /// <summary>
        /// SetValue Delegate CallBack 및 OldValue Dispose
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue"></param>
        /// <param name="disposeOldValue"></param>
        /// <param name="raiseChangedDelegate"></param>
        /// <param name="oldValue"></param>
        /// <param name="propertyName"></param>
        private void PostCallback<T>(T newValue, bool disposeOldValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate, T oldValue, [CallerMemberName] string propertyName = "")
        {
            NotifyPropertyChanged(propertyName);
            if (raiseChangedDelegate != null)
                raiseChangedDelegate(oldValue, newValue);
            if (!disposeOldValue) return;
            IDisposable disposableOldValue = oldValue as IDisposable;
            if (disposableOldValue != null)
                disposableOldValue.Dispose();
        }

        #endregion SetValue
    }
}
