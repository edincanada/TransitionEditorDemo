
namespace XLib.ScreenMgmt.Transitions
{
   public interface IScreenTransition
   {  string TransitionName { get; }
      string TransitionInName { get; }
      string TransitionOutName { get; }
      bool Simultaneous { get; }
   }
}