
namespace XLib.ViewMgmt.Transitions
{
   public interface IViewTransition
   {  string TransitionName { get; }
      string TransitionInName { get; }
      string TransitionOutName { get; }
      bool Simultaneous { get; }
   }
}