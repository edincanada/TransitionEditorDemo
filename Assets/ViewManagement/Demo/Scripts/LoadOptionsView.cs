
using UnityEngine;

namespace XLib.Demo
{
   using Util;
   public class LoadOptionsView : XLib.ViewMgmt.LoadView
   {
      public static readonly string BACKGROUND_SPRITE_KEY = "background_sprite";
      [SerializeField]
      Sprite _backgroundSprite = default;

      public override void DoLoad()
      {  StringKeyDictionary dict = new StringKeyDictionary();
         dict.Add(BACKGROUND_SPRITE_KEY, _backgroundSprite);
         DoLoadImpl(dict, false);
      }
   }
}
