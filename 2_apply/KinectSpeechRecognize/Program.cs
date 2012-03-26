using System;
using System.IO;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace KinectSpeechRecognize
{
  class Program
  {
    static void Main( string[] args )
    {
      try {
        // Kinectが接続されているかどうかを確認する
        if ( KinectSensor.KinectSensors.Count == 0 ) {
          throw new Exception( "Kinectを接続してください" );
        }

        // 認識器の一覧を表示し、使用する認識器を取得する
        ShowRecognizer();
        //RecognizerInfo info = GetRecognizer( "en-US" );
        RecognizerInfo info = GetRecognizer( "ja-JP" );
        Console.WriteLine( "Using: {0}", info.Name );

        // 認識させる単語を登録する
        Choices colors = new Choices();
        colors.Add( "red" );
        colors.Add( "green" );
        colors.Add( "blue" );
        colors.Add( "赤" );
        colors.Add( "ミドリ" );
        colors.Add( "あお" );

        // 文法の設定を行う
        GrammarBuilder builder = new GrammarBuilder();
        builder.Culture = info.Culture;
        builder.Append( colors );
        Grammar grammar = new Grammar( builder );

        // 認識エンジンの設定と、単語が認識されたときの通知先の登録を行う
        SpeechRecognitionEngine engine = new SpeechRecognitionEngine( info.Id );
        engine.LoadGrammar( grammar );
        engine.SpeechRecognized +=
          new EventHandler<SpeechRecognizedEventArgs>( engine_SpeechRecognized );

        // Kinectの動作を開始する
        KinectSensor kinect = KinectSensor.KinectSensors[0];
        kinect.Start();

        // 音声のインタフェースを取得し、動作を開始する
        KinectAudioSource audio = kinect.AudioSource;
        using ( Stream s = audio.Start() ) {
          // 認識エンジンに音声ストリームを設定する
          engine.SetInputToAudioStream( s, new SpeechAudioFormatInfo(
                                          EncodingFormat.Pcm, 16000, 16, 1,
                                          32000, 2, null ) );

          Console.WriteLine( "Recognizing. Press ENTER to stop" );

          // 非同期で、音声認識を開始する
          engine.RecognizeAsync( RecognizeMode.Multiple );
          Console.ReadLine();
          Console.WriteLine( "Stopping recognizer ..." );

          // 音声認識を停止する
          engine.RecognizeAsyncStop();
        }
      }
      catch ( Exception ex ) {
        Console.WriteLine( ex.Message );
      }
    }

    /// <summary>
    /// 単語を認識した時に通知される
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    static void engine_SpeechRecognized( object sender, SpeechRecognizedEventArgs e )
    {
      Console.WriteLine( "\nSpeech Recognized: \tText:{0}, Confidence:{1}",
        e.Result.Text, e.Result.Confidence );
    }

    /// <summary>
    /// 認識器の一覧を表示する
    /// </summary>
    static void ShowRecognizer()
    {
      foreach ( var recognizer in SpeechRecognitionEngine.InstalledRecognizers() ) {
        Console.WriteLine( string.Format("{0}, {1}", recognizer.Culture.Name, recognizer.Name ) );
      }

      Console.WriteLine( "" );
    }

    /// <summary>
    /// 指定した認識器を取得する(Cultureの名前で選択する)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static RecognizerInfo GetRecognizer( string name )
    {
      Func<RecognizerInfo, bool> matchingFunc = r =>
      {
        return name.Equals( r.Culture.Name, StringComparison.InvariantCultureIgnoreCase );
      };

      return SpeechRecognitionEngine.InstalledRecognizers().Where( matchingFunc ).FirstOrDefault();
    }
  }
}
