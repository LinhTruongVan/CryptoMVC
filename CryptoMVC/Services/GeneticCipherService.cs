using System;
using System.IO;
using System.Text;

namespace CryptoMVC.Services
{
    public class GeneticCipherService
    {
        // matrix 5*5
        public const int MATRIX = 25; // ma trận sử dụng để tạo key và mã hóa
        public const int PRIME_A = 7; // đối số để thực hiện phép thay thế  
        public const int PRIME_B = 2; // đối số để thực hiện phép thay thế
        public const int CROSS_A = 3; // đối số để thực hiện phép cross-over
        public const int CROSS_B = 10; // đối số để thực hiện phép cross-over
        public const int BYTE_MAX = 256;

        byte[] magic = new byte[12] { 127, 1, 127, 0, 127, 1, 127, 0, 127, 1, 127, 0 }; // magic byte array dùng để xác định điểm kết thúc của 1 file

        public byte[] Magic
        {
            get { return magic; }
            set { magic = value; }
        }

        int padMatrix;


        public GeneticCipherService()
        {
            padMatrix = magic.Length / MATRIX + 1; // kích thước byte để đệm vào để đủ kích thước của Matrix
        }

        //
        //For testing a simple case
        //
        public void test()
        {
            string pass = "123456";
            string plainText = "Độc lập tài chính quan trọng vì nó là chìa khóa mở ra nhiều cánh cửa. Có người nói rằng nếu bạn yêu công việc của mình";
            byte[] plain = Encoding.UTF8.GetBytes(plainText);
            byte[] key = GenerateKeyMatrix(pass);
            byte[] a = SubstitutionEncrypt(plain, key);

            byte[] b = SubstitutionDecrypt(a, key);

            byte[] c = GeneticFunctionEncrypt(a);

            byte[] d = GeneticFunctionDecrypt(c);


        }

        //
        //Generate a matrix from string
        //input: string
        //output: byte[]
        // khởi tạo ma trận khóa
        public byte[] GenerateKeyMatrix(string password)
        {
            byte[] a = new byte[25];
            byte[] b = Encoding.ASCII.GetBytes(password); // get mảng byte của string password
            if (b.Length < MATRIX) // nếu kích thước của mảng password nhỏ hơn kích thước matrix
            {
                for (int i = b.Length; i < MATRIX; i++)  // chèn vào cho đủ kích thước matrix với các kí tự z
                {
                    a[i] = 122; // z char = 122 
                }
                Array.Copy(b, a, b.Length);
            }
            else
            {
                Array.Copy(b, a, MATRIX); // còn không thì lấy đủ kích thước matrix
            }

            for (int i = 0; i < MATRIX; i++)
            {
                a[i] = (byte)(a[i] >> 2); // bước xử lý ma trận khóa, dịch phải qua 2 vị trí
            }

            return a; // trả về mảng ma trận khóa
        }

        //
        //Handle substitution encrypt
        //input: matrix of plain text & matrix of key
        //output: immediate cipher text
        // thực hiện phép thay thế
        public byte[] SubstitutionEncrypt(byte[] a, byte[] passMatrix) // mảng byte a là mảng byte của plain text
        {
            int n = a.Length;
            byte[] b;

            int pad = MATRIX - (n % MATRIX); // tính toán kích thước cần chèn thêm cho đủ chiều dài matrix 
            int m;
            if (pad >= magic.Length)
            {
                m = n / MATRIX + padMatrix; // nếu kích thước chèn thêm lớn hơn mảng magic
            }
            else
            {
                m = n / MATRIX + padMatrix + 1; // nếu kích thước chèn them nhỏ hơn mảng magic thì cần tăng thêm 1 matrix
            }
            // m là số lượng matrix đủ để chứa mảng byte của plain text và mảng magic để xác định vị trí kết thúc
            // nếu mảng byte của plain text ko chia hết cho matrix thì cần chèn thêm các byte 00
            b = new byte[m * MATRIX];
            Array.Copy(a, b, n);
            int count = 0;
            for (int i = n; i < m * MATRIX; i++)
            {
                if (count < magic.Length)
                {
                    b[i] = magic[count];
                    count++; // chèn thêm magic byte vào cuối file
                }
                else
                {
                    b[i] = 0;
                }
            }


            for (int i = 0; i < b.Length; i++)
            {
                int tempa = b[i];
                int s = tempa + passMatrix[i % MATRIX]; // thực hiện mã hóa
                s = ((s * PRIME_A + PRIME_B) % BYTE_MAX);// thực hiên phép thay thế với prime A và Prime B

                b[i] = (byte)s;
            }

            return b;
        }

        //
        //Handle cross-over and mutation
        //input: matrix of immediate cipher text
        //output: final cipher text as bytes
        // Mã hóa xử dụng các phương thức genetic
        public byte[] GeneticFunctionEncrypt(byte[] t)
        {
            byte[] byteStream = new byte[t.Length];
            Array.Copy(t, byteStream, t.Length);
            int n = byteStream.Length / MATRIX;
            for (int i = 0; i < n; i++)
            {
                for (int j = (i * MATRIX + CROSS_A); j < (i * MATRIX + CROSS_B); j++)
                {
                    BinaryOperator.SwapElement(ref byteStream[j], ref byteStream[j + MATRIX / 2]); // đổi chỗ 2 vị trí trong mảng dựa vào vị trí A và B

                }
            }

            for (int i = 0; i < byteStream.Length; i++)
            {
                byteStream[i] = BinaryOperator.ReverseByte(byteStream[i]); // nghịch đảo các giá trị trong mảng byte
            }
            return byteStream;
        }

        //
        //Handle substitution decrypt
        //input: matrix of final cipher text & matrix of key
        //output: matrix of immediate cipher text
        // giải mả xử dụng phương thức thay thế
        public byte[] SubstitutionDecrypt(byte[] t, byte[] passMatrix)
        {
            byte[] a = new byte[t.Length];
            Array.Copy(t, a, t.Length); // tạo mảng copy
            for (int i = 0; i < a.Length; i++)
            {
                byte te = a[i];
                a[i] = FindByteBeforeSub(a[i]); // hàm tìm kiếm giá trị byte trước khi thực hiện phép thay thế 
                a[i] -= passMatrix[i % MATRIX]; // giải mã xử dụng ma trận khóa
            }
            int match = 0;
            int index = -1;
            for (int i = a.Length - 1; i >= a.Length - 1 - (padMatrix + 1) * MATRIX; i--) // kiểm tra từ cuối file, tìm kiếm mảng magic để xác định vị trí cuối
            {                                                                               // của file và xóa đi các byte 00 chèn thêm 
                int k = i;
                for (int j = magic.Length - 1; j >= 0; j--, k--)
                {
                    if (a[k] == magic[j]) // nếu giá trị trong file bằng với giá trị của mảng magic
                    {
                        match++; // tăng match
                        if (match == magic.Length) // match = chiều dài magic thì đã xác định được vị trí cuối file
                        {
                            index = k;
                            break;
                        }
                    }
                    else
                    {
                        match = 0;
                    }
                }
                if (index != -1)
                    break;
            }

            if (index != -1)
            {
                byte[] plainStream = new byte[index]; // copy mảng byte của file gốc và loại bỏ đi các byte chèn thêm
                Array.Copy(a, plainStream, index);
                return plainStream;
            }

            return a;

        }

        public byte FindByteBeforeSub(byte a) // thuật toán tìm kiếm giá trị trước khi thực hiện phép thay thế
        {
            for (int i = 0; i <= BYTE_MAX; i++)
            {
                int t = a + BYTE_MAX * i; // các giá trị có thể sau khi thực hiện phép thay thế
                if ((t - PRIME_B) % PRIME_A == 0) // kiểm tra lại
                {
                    return (byte)((t - PRIME_B) / PRIME_A);
                }
            }
            return 0;

        }

        //
        //Handle descrypt cross-over and mutation
        //input: final cipher text as bytes
        //output: immediate cipher text as bytes
        // thực hiện giải mã bằng các phép genetic
        public byte[] GeneticFunctionDecrypt(byte[] t)
        {
            byte[] byteStream = new byte[t.Length];
            Array.Copy(t, byteStream, t.Length);
            for (int i = 0; i < byteStream.Length; i++)
            {
                byteStream[i] = BinaryOperator.ReverseByte(byteStream[i]); // dịch ngược giá trị của mảng byte
            }

            int n = byteStream.Length / MATRIX;
            for (int i = 0; i < n; i++)
            {
                for (int j = (i * MATRIX + CROSS_A); j < (i * MATRIX + CROSS_B); j++)
                {
                    BinaryOperator.SwapElement(ref byteStream[j], ref byteStream[j + MATRIX / 2]); // đảo vị trí của giá trị troang mảng dựa vào cross A và B

                }
            }
            return byteStream;

        }


        #region Encrypt Descrypt for byte[]
        public byte[] Encrypt(byte[] byteStream, string password)
        {
            byte[] temp = new byte[byteStream.Length];
            Array.Copy(byteStream, temp, byteStream.Length);
            byte[] passMatrix = GenerateKeyMatrix(password);
            byte[] subStream = SubstitutionEncrypt(temp, passMatrix);
            byte[] cipherStream = GeneticFunctionEncrypt(subStream);
            return cipherStream;

        }

        public byte[] Decrypt(byte[] byteStream, string password)
        {
            byte[] temp = new byte[byteStream.Length];
            Array.Copy(byteStream, temp, byteStream.Length);
            byte[] passMatrix = GenerateKeyMatrix(password);
            byte[] subStream = GeneticFunctionDecrypt(temp);
            byte[] plainStream = SubstitutionDecrypt(subStream, passMatrix);
            return plainStream;
        }
        #endregion

        #region Encrypt Descrypt for file
        public bool EncryptFile(string plain, string cipher, string password = "!@#$abc123")
        {
            try
            {
                byte[] cipherStream = Encrypt(File.ReadAllBytes(plain), password);
                File.WriteAllBytes(cipher, cipherStream);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DecryptFile(string cipher, string plain, string password = "!@#$abc123")
        {
            try
            {
                byte[] plainStream = Decrypt(File.ReadAllBytes(cipher), password);
                File.WriteAllBytes(plain, plainStream);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Encrypt Descrypt for string
        public string EncryptString(string plain, string password = "!@#$abc123")
        {
            try
            {
                byte[] cipherStream = Encrypt(UTF8Encoding.UTF8.GetBytes(plain), password);
                return UTF8Encoding.UTF8.GetString(cipherStream);
            }
            catch
            {

                return null;
            }
        }

        public string DecryptString(string cipher, string password = "!@#$abc123")
        {
            try
            {
                byte[] plainStream = Decrypt(UTF8Encoding.UTF8.GetBytes(cipher), password);
                return UTF8Encoding.UTF8.GetString(plainStream);
            }
            catch
            {

                return null;
            }
        }
        #endregion
    }

    public class BinaryOperator
    {
        public const int BYTE_LENGTH = 8;
        //
        // Convert byte to array of binary. Example:  4 <=> 0000 0100
        //
        public static byte[] ConvertByteToBinary(byte a)
        {
            byte[] b = new byte[8];
            for (int i = BYTE_LENGTH - 1; i >= 0; i--)
            {
                b[i] = (byte)(a % 2);
                a /= 2;
            }
            return b;
        }

        public static byte ConvertBinaryToByte(byte[] b)
        {
            byte a = 0;
            for (int i = BYTE_LENGTH - 1; i >= 0; i--)
            {
                a += (byte)(b[i] * Math.Pow(2, i));
            }
            return a;
        }
        public static byte ReverseByte(byte a)
        {
            byte[] temp = ConvertByteToBinary(a);
            for (int i = 0; i < BYTE_LENGTH; i++)
            {
                temp[i] = (byte)(1 - temp[i]);
            }
            byte result = ConvertBinaryToByte(temp);
            return result;
        }
        public static void SwapElement(ref byte a, ref byte b)
        {
            byte t = a;
            a = b;
            b = t;
        }
    }
}