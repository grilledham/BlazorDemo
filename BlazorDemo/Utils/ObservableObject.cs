using System.Runtime.CompilerServices;

namespace BlazorDemo.Utils
{
    public interface IPropertyChanged<T>
    {
        public event EventHandler<T, string> PropertyChanged;
    }

    public class ObservableObject<T> : IPropertyChanged<T> where T : ObservableObject<T>
    {
        public event EventHandler<T, string> PropertyChanged;

        protected bool SetAndNotify<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = "")
        {
            if (field.Equals(value))
            {
                return false;
            }

            field = value;
            Raise(propertyName);
            return true;
        }

        protected void Raise(string name)
        {
            PropertyChanged?.Invoke((T)this, name);
        }
    }
}
