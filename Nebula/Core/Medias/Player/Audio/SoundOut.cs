using System;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;

namespace Nebula.Core.Medias.Player.Audio
{
    public class SoundOut
    {
        public ISoundOut   Out     { get; private set; }
        public IWaveSource Source  { get; private set; }
        public bool        IsReady => Out != null && Source != null;

        public void Prepare(Uri uri)
        {
            CleanUp();
            Source = CodecFactory.Instance.GetCodec(uri);
            Out = new WasapiOut();
            Out.Initialize(Source);
        }

        private void CleanUp()
        {
            if (Out != null)
            {
                Out.Dispose();
                Out = null;
            }

            if (Source != null)
            {
                Source.Dispose();
                Source = null;
            }
        }
    }
}