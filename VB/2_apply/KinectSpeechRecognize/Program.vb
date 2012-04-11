Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.Kinect
Imports Microsoft.Speech.AudioFormat
Imports Microsoft.Speech.Recognition
Module Program

    Sub Main(args As String())
        Try
            ' Kinectが接続されているかどうかを確認する
            If KinectSensor.KinectSensors.Count = 0 Then
                Throw New Exception("Kinectを接続してください")
            End If

            ' 認識器の一覧を表示し、使用する認識器を取得する
            Call ShowRecognizer()
            'RecognizerInfo info = GetRecognizer( "en-US" );
            Dim info As RecognizerInfo = GetRecognizer("ja-JP")
            Console.WriteLine("Using: {0}", info.Name)

            ' 認識させる単語を登録する
            Dim colors = New Choices
            colors.Add("red")
            colors.Add("green")
            colors.Add("blue")
            colors.Add("赤")
            colors.Add("ミドリ")
            colors.Add("あお")

            ' 文法の設定を行う
            Dim builder = New GrammarBuilder()
            builder.Culture = info.Culture
            builder.Append(colors)
            Dim _grammar = New Grammar(builder)

            ' 認識エンジンの設定と、単語が認識されたときの通知先の登録を行う
            Dim engine = New SpeechRecognitionEngine(info.Id)
            engine.LoadGrammar(_grammar)
            AddHandler engine.SpeechRecognized, AddressOf engine_SpeechRecognized

            ' Kinectの動作を開始する
            Dim kinect As KinectSensor = KinectSensor.KinectSensors(0)
            kinect.Start()

            ' 音声のインタフェースを取得し、動作を開始する
            Dim audio As KinectAudioSource = kinect.AudioSource
            Using s As Stream = audio.Start
                ' 認識エンジンに音声ストリームを設定する
                engine.SetInputToAudioStream(s, New SpeechAudioFormatInfo(
                                                EncodingFormat.Pcm,
                                                16000,
                                                16,
                                                1,
                                                32000,
                                                2,
                                                Nothing))

                Console.WriteLine("Recognizing. Press ENTER to stop")

                ' 非同期で、音声認識を開始する
                engine.RecognizeAsync(RecognizeMode.Multiple)
                Console.ReadLine()
                Console.WriteLine("Stopping recognizer ...")

                ' 音声認識を停止する
                engine.RecognizeAsyncStop()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 単語を認識した時に通知される
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub engine_SpeechRecognized(sender As Object, e As SpeechRecognizedEventArgs)
        Console.WriteLine("\nSpeech Recognized: \tText:{0}, Confidence:{1}",
                          e.Result.Text,
                          e.Result.Confidence)
    End Sub

    ''' <summary>
    ''' 認識器の一覧を表示する
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowRecognizer()
        For Each recognizer As RecognizerInfo In SpeechRecognitionEngine.InstalledRecognizers
            Console.WriteLine(String.Format("{0}, {1}", recognizer.Culture.Name, recognizer.Name))
        Next

        Console.WriteLine("")
    End Sub

    ''' <summary>
    ''' 指定した認識器を取得する(Cultureの名前で選択する)
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetRecognizer(name As String) As RecognizerInfo

        Dim matchingFunc As Func(Of RecognizerInfo, Boolean) = Function(r)
                                                                   Return name.Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase)
                                                               End Function
        Return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault()
    End Function
End Module
