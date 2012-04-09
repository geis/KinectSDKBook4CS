// StreamingWavePlayer.h

#pragma once

#include <Windows.h>
#include <mmsystem.h>
#pragma comment( lib, "winmm.lib" )

using namespace System;
using namespace System::Runtime::InteropServices;

namespace Geis {
  namespace Win32 {
    public ref class StreamingWavePlayer
    {
    public:

      StreamingWavePlayer( int nSampleRate, int nBitsPerSample, Byte nChannels, int audioBufferCount )
      {
        audioBufferCount_ = audioBufferCount;
        audioBufferNextIndex_ = 0;

        // WAVEデータの設定
        WAVEFORMATEX wf;
        wf.wFormatTag = 0x0001; // PCM
        wf.nChannels = nChannels;
        wf.nSamplesPerSec = nSampleRate;
        wf.wBitsPerSample = nBitsPerSample;
        wf.nBlockAlign = wf.wBitsPerSample * wf.nChannels / 8;
        wf.nAvgBytesPerSec = wf.nBlockAlign * wf.nSamplesPerSec;

        handle_ = new HWAVEOUT;
        MMRESULT mmRes = ::waveOutOpen( handle_, WAVE_MAPPER, &wf, NULL, NULL, CALLBACK_NULL );
        if ( mmRes != MMSYSERR_NOERROR ) {
          throw gcnew Exception( "waveOutOpen failed\n" );
        }

        // 音声データ用のバッファの作成と初期化
        waveHeaders_ = new WAVEHDR[audioBufferCount_];
        memset( &waveHeaders_[0], 0, sizeof(WAVEHDR) * audioBufferCount_ );

        for ( int i = 0; i < audioBufferCount_; ++i ) {
          waveHeaders_[i].lpData = new char[MAX_BUFFER_SIZE];
          waveHeaders_[i].dwUser = i;
          waveHeaders_[i].dwFlags = WHDR_DONE;
        }
      }

      !StreamingWavePlayer()
      {
        ::waveOutPause( *handle_ );

        for ( int i = 0; i < audioBufferCount_; ++i ) {
          delete[] waveHeaders_[i].lpData;
        }

        delete waveHeaders_;
        delete handle_;
      }

      ~StreamingWavePlayer()
      {
        this->!StreamingWavePlayer();
      }

      void Output( IntPtr buffer, int length )
      {
        // バッファの取得
        WAVEHDR* pHeader = &waveHeaders_[audioBufferNextIndex_];
        if ( (pHeader->dwFlags & WHDR_DONE) == 0 ) {
          return;
        }

        // WAVEデータの取得
        pHeader->dwBufferLength = length;
        pHeader->dwFlags = 0;
        CopyMemory( pHeader->lpData, (void*)buffer, length );

        Output( pHeader );
      }

      void Output( array<Byte>^ buffer )
      {
        WAVEHDR* pHeader = GetBuffer();
        if ( pHeader == 0 ) {
          return;
        }

        // WAVEデータの取得
        pHeader->dwBufferLength = buffer->Length;
        pHeader->dwFlags = 0;
        Marshal::Copy( buffer, 0, (IntPtr)pHeader->lpData, buffer->Length );

        Output( pHeader );
      }

    private:

      WAVEHDR* GetBuffer()
      {
        // バッファの取得
        WAVEHDR* pHeader = &waveHeaders_[audioBufferNextIndex_];
        if ( (pHeader->dwFlags & WHDR_DONE) == 0 ) {
          return 0;
        }

        // WAVEヘッダのクリーンアップ
        MMRESULT mmRes = ::waveOutUnprepareHeader( *handle_, pHeader, sizeof(WAVEHDR) );
        if ( mmRes != MMSYSERR_NOERROR ) {
          return 0;
        }

        return pHeader;
      }

      void Output( WAVEHDR* pHeader ) {
        // WAVEヘッダの初期化
        MMRESULT mmRes = ::waveOutPrepareHeader( *handle_, pHeader, sizeof(WAVEHDR) );
        if ( mmRes != MMSYSERR_NOERROR ) {
          return;
        }

        // WAVEデータを出力キューに入れる
        mmRes = ::waveOutWrite( *handle_, pHeader, sizeof(WAVEHDR) );
        if ( mmRes != MMSYSERR_NOERROR ) {
          return;
        }

        // 次のバッファインデックス
        audioBufferNextIndex_ = (audioBufferNextIndex_ + 1) % audioBufferCount_;
      }

    private:

      static const int MAX_BUFFER_SIZE = 2 * 1024 * 1024;

      HWAVEOUT* handle_;
      WAVEHDR* waveHeaders_;

      int audioBufferCount_;
      int audioBufferNextIndex_;
    };
  }
}
