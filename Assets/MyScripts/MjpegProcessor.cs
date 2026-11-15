using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
//https://github.com/DanielArnett/SampleUnityMjpegViewer
using System.Drawing;
using UnityEngine.Networking;

public class MjpegProcessor: MonoBehaviour {

    // 2 byte header for JPEG images
    private readonly byte[] jpegHeader = new byte[] { 0xff, 0xd8 };
    // pull down 1024 bytes at a time
    private int _chunkSize = 1024*4;
    // used to cancel reading the stream
    private bool _streamActive;
    // current encoded JPEG image
    public byte[] currentFrame { get; private set; }
    // WPF, Silverlight
    //public BitmapImage BitmapImage { get; set; }
    // used to marshal back to UI thread
    private SynchronizationContext _context;
    public byte[] latestFrame = null;
    private bool responseReceived = false;
    private Uri lastUri;
    private string lastUsername;
    private string lastPassword;

    private WebRequest request;
    private byte[] postBytes;

    // event to get the buffer above handed to you
    public event EventHandler<FrameReadyEventArgs> FrameReady;
    public event EventHandler<ErrorEventArgs> Error;

    public MjpegProcessor(int chunkSize = 4 * 1024)
    {
        _context = SynchronizationContext.Current;
        _chunkSize = chunkSize;
    }

    public void ParseStream(Uri uri, string username, string password)
    {   

        Debug.Log("Parsing Stream " + uri.ToString());
        request = WebRequest.Create(uri);
        request.Method = "POST";
        request.ContentType = "application/json; charset = utf-8";
        postBytes = Encoding.UTF8.GetBytes ( "{'name': 'camera.getLivePreview'}");
        request.ContentLength = postBytes.Length;
        //request.Timeout = 2000;

        
        if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            request.Credentials = new NetworkCredential(username, password);
        // asynchronously get a response
        Thread clientReceiveThread = new Thread (new ThreadStart(MakeRequest)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  
        
    }
    void MakeRequest() {
        try {
            Stream reqStream = request.GetRequestStream ();
            reqStream.Write (postBytes, 0, postBytes.Length) ;
            reqStream.Close ();
            //request.BeginGetResponse(OnPostResponse, request);
            Debug.Log("SENDING REQUEST");
            using (var response = request.GetResponse())
            {
                Debug.Log("GOT RESPONSE");
                Stream dataStream = response.GetResponseStream();
                Debug.Log("got stream");
                StreamReader reader = new StreamReader(dataStream);
                string MSG = reader.ReadLine();
                Debug.Log(MSG);
            }
            

        }
        catch (WebException ex)
        {
            Debug.Log("WEB EXCEPTION");
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                Debug.Log(reader.ReadToEnd());
            }
        }
        catch(Exception e)
        {
            Debug.Log("ERROR");
            Debug.Log(e);
        }
    }

    public void StopStream()
    {
        _streamActive = false;
    }
    public static int FindBytes(byte[] buff, byte[] search)
    {
        // enumerate the buffer but don't overstep the bounds
        for (int start = 0; start < buff.Length - search.Length; start++)
        {
            // we found the first character
            if (buff[start] == search[0])
            {
                int next;

                // traverse the rest of the bytes
                for (next = 1; next < search.Length; next++)
                {
                    // if we don't match, bail
                    if (buff[start + next] != search[next])
                        break;
                }

                if (next == search.Length)
                    return start;
            }
        }
        // not found
        return -1;
    }

    private void OnPostResponse(IAsyncResult asyncResult)
    {
        responseReceived = true;
        Debug.Log("OnPostResponse");
        byte[] imageBuffer = new byte[1024 * 1024];

        Debug.Log("Starting request");
        // get the response
        HttpWebRequest req = (HttpWebRequest)asyncResult.AsyncState;

        try
        {
            Debug.Log("OnPostResponse try entered.");
            HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(asyncResult);
            Debug.Log("response received");
            // find our magic boundary value
            string contentType = resp.Headers["Content-Type"];
            if (!string.IsNullOrEmpty(contentType) && !contentType.Contains("="))
            {
                Debug.Log("MJPEG Exception thrown");
                throw new Exception("Invalid content-type header.  The camera is likely not returning a proper MJPEG stream.");
            }

            string boundary = resp.Headers["Content-Type"].Split('=')[1].Replace("\"", "");
            byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

            Stream s = resp.GetResponseStream();
            BinaryReader br = new BinaryReader(s);

            _streamActive = true;
            byte[] buff = br.ReadBytes(_chunkSize);

            while (_streamActive)
            {
                // find the JPEG header
                int imageStart = FindBytes(buff, jpegHeader);// buff.Find(jpegHeader);

                if (imageStart != -1)
                {
                    // copy the start of the JPEG image to the imageBuffer
                    int size = buff.Length - imageStart;
                    Array.Copy(buff, imageStart, imageBuffer, 0, size);

                    while (true)
                    {
                        buff = br.ReadBytes(_chunkSize);

                        // Find the end of the jpeg
                        int imageEnd = FindBytes(buff, boundaryBytes);
                        if (imageEnd != -1)
                        {
                            // copy the remainder of the JPEG to the imageBuffer
                            Array.Copy(buff, 0, imageBuffer, size, imageEnd);
                            size += imageEnd;

                            // Copy the latest frame into `CurrentFrame`
                            byte[] frame = new byte[size];
                            Array.Copy(imageBuffer, 0, frame, 0, size);
                            currentFrame = frame;

                            // tell whoever's listening that we have a frame to draw
                            if (FrameReady != null)
                                FrameReady(this, new FrameReadyEventArgs());
                            // copy the leftover data to the start
                            Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

                            // fill the remainder of the buffer with new data and start over
                            byte[] temp = br.ReadBytes(imageEnd);

                            Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
                            break;
                        }

                        // copy all of the data to the imageBuffer
                        Array.Copy(buff, 0, imageBuffer, size, buff.Length);
                        size += buff.Length;

                        if (!_streamActive)
                        {
                            Debug.Log("CLOSING");
                            resp.Close();
                            break;
                        }
                    }
                }
            }
            resp.Close();
        }
        catch (Exception ex)
        {
            if (Error != null)
                _context.Post(delegate { Error(this, new ErrorEventArgs() { Message = ex.Message }); }, null);
                Debug.Log("retrying");
                //ParseStream(lastUri, lastUsername, lastPassword);
            return;
        }
    }
}

public class FrameReadyEventArgs : EventArgs
{
  
}

public sealed class ErrorEventArgs : EventArgs

{
    public string Message { get; set; }
    public int ErrorCode { get; set; }
}