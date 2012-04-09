#pragma once

#include <Windows.h>

#pragma comment( lib, "user32.lib" )

#using <System.Drawing.dll>
#using <System.Windows.Forms.dll>

using namespace System::Drawing;
using namespace System::Windows;
using namespace System::Windows::Forms;

namespace Geis {
  namespace Win32 {
    public ref class SendInput
    {
    public:
      SendInput()
      {
      }

      static void MouseMove( int x, int y, Screen^ screen )
      {
        INPUT input = { 0 };
        input.type = INPUT_MOUSE;
        input.mi.dx = (LONG)((x * 65535) / screen->Bounds.Width);
        input.mi.dy = (LONG)((y * 65535) / screen->Bounds.Height);
        input.mi.dwFlags = MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE;

        ::SendInput( 1, &input, sizeof(input) );
      }

      static void LeftClick()
      {
        INPUT input[] = {
          { INPUT_MOUSE, 0, 0, 0, MOUSEEVENTF_LEFTDOWN },
          { INPUT_MOUSE, 0, 0, 0, MOUSEEVENTF_LEFTUP },
        };

        ::SendInput( sizeof(input) / sizeof(input[0]), input, sizeof(input[0]) );
      }
    };
  }
}
