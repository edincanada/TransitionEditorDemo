
sealed public class CoroutineLock : System.IDisposable
{
   bool _locked = false;

   public CoroutineLock Lock()
   {  _locked = true;
      return this;
   }

   public void Unlock() { _locked = false; }

   public void Dispose() { Unlock(); }

   public bool IsLocked { get { return _locked; } }
}
