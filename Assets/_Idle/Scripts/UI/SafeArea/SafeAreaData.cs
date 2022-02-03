using System;
using UnityEngine;

namespace _Idle.Scripts.UI.SafeArea
{
    public enum DeviceType
    {
        None,
        iPhoneX,
        iPhoneXsMax,
        Pixel3XL_LSL,
        Pixel3XL_LSR,
        XiaomiMi8Se,
        HuaweiP20
    }
    
    public class SafeAreaData
    {
        private static readonly Rect[] _iPhoneX = {
            new Rect(0f, 102f / 2436f, 1f, 2202f / 2436f), // Portrait
            new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f) // Landscape
        };

        private static readonly Rect[] _iPhoneXR = {
            new Rect(0f, 102f / 2436f, 1f, 2202f / 2436f), // Portrait
            new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f) // Landscape
        };
        
        private static readonly Rect[] _iPhoneXsMax = {
            new Rect(0f, 102f / 2688f, 1f, 2454f / 2688f),
            new Rect(132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)
        };

        private static readonly Rect[] _pixel3XlLsl = {
            new Rect(0f, 0f, 1f, 2789f / 2960f),
            new Rect(0f, 0f, 2789f / 2960f, 1f)
        };

        private static readonly Rect[] _pixel3XlLsr = {
            new Rect(0f, 0f, 1f, 2789f / 2960f),
            new Rect(171f / 2960f, 0f, 2789f / 2960f, 1f)
        };
        
                
        private static readonly Rect[] _xiaomiMi8Se = {
            new Rect(0f, 102f / 2244f, 1f, 2202f / 2244f),
            new Rect(0f, 102f / 2244f, 1f, 2202f / 2244f)
        };
        
        private static readonly Rect[] _huaweiP20 = {
            new Rect(0f, 102f / 2436f, 1f, 2250f / 2436f), // Portrait
            new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f) // Landscape
        };

        public static Rect Get(DeviceType sim)
        {
            Rect nsa;
            
            switch (sim)
            {
                case DeviceType.iPhoneX:
                    nsa = Screen.height > Screen.width ? _iPhoneX[0] : _iPhoneX[1];
                    break;
                
                case DeviceType.iPhoneXsMax:
                    nsa = Screen.height > Screen.width ? _iPhoneXsMax[0] : _iPhoneXsMax[1];
                    break;
                
                case DeviceType.Pixel3XL_LSL:
                    nsa = Screen.height > Screen.width ? _pixel3XlLsl[0] : _pixel3XlLsl[1];
                    break;
                
                case DeviceType.Pixel3XL_LSR:
                    nsa = Screen.height > Screen.width ? _pixel3XlLsr[0] : _pixel3XlLsr[1];
                    break;
                
                case DeviceType.XiaomiMi8Se:
                    nsa = Screen.height > Screen.width ? _xiaomiMi8Se[0] : _xiaomiMi8Se[1];
                    break;
               
                case DeviceType.HuaweiP20:
                    nsa = Screen.height > Screen.width ? _xiaomiMi8Se[0] : _xiaomiMi8Se[1];
                    break;
                
                case DeviceType.None:
                    if (Screen.height == 2244)
                    {
                        nsa = Screen.height > Screen.width ? _huaweiP20[0] : _huaweiP20[1];
                        break;
                    }
                    
                    return Screen.safeArea;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(sim), sim, null);
            }
            
            return new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width,Screen.height * nsa.height);
        }
    }
}