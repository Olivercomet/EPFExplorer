using System.IO;
using System.Text;

namespace Hifss.Extensions
{
    internal class ApplicationExtension : Extension
    {
        private const int IDENTIFIER_LENGTH = 8;
        private const int AUTH_LENGTH = 3;
        private string _application;
        private string _auth;
        private byte[] _appData;

        public override bool Read(Stream stream)
        {
            //Skipping application identificator length since it's known (1 byte)
            stream.Seek(1, SeekOrigin.Current);

            readAppIdentifier(stream);
            readAuth(stream);
            readAppData(stream);

            return true;
        }

        private void readAppData(Stream stream)
        {
            int dataLength = stream.ReadByte();
            _appData = new byte[dataLength];

            stream.Read(_appData, 0, dataLength);
        }

        private void readAuth(Stream stream)
        {
            byte[] authBuffer = new byte[AUTH_LENGTH];
            stream.Read(authBuffer, 0, AUTH_LENGTH);

            _auth = Encoding.ASCII.GetString(authBuffer);
        }

        private void readAppIdentifier(Stream stream)
        {
            byte[] identBuffer = new byte[IDENTIFIER_LENGTH];
            stream.Read(identBuffer, 0, IDENTIFIER_LENGTH);

            _application = Encoding.ASCII.GetString(identBuffer);
        }
    }
}
