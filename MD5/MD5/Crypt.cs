using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD5
{
    public class Crypt
    {
        //велечина битового сдвига для раунда
        int[] s = {    7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
                        5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
                        4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
                        6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21 };

        //белый шум для хэша
        uint[] K = {0,0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
                    0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
                    0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
                    0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
                    0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
                    0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
                    0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
                    0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
                    0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
                    0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
                    0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
                    0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
                    0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
                    0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
                    0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
                    0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391 };

        //тут будут храниться блоки
        uint[] block;

        //встроенная функция md5
        public   string etalon_md5(string input_string)
        {
            byte[] input = System.Text.Encoding.ASCII.GetBytes(input_string);
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBytes = md5.ComputeHash(input);
            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        //моя md5
        public string my_md5(string input_string)
        {
            byte[] input = System.Text.Encoding.ASCII.GetBytes(input_string);       //преобразуем входную строку в битовый масиив
            byte[] input_plus;
            int counter = 0;
            if (input.Length % 64 < 56)                                             //в зависимости от длины исходной длины сообщения инициализируем наш расширенный массив разной длины
                input_plus = new byte[(input.Length / 64 + 1) * 64];                //если крайний блок длины меньше 56, то нам досточно блоков
            else
                input_plus = new byte[(input.Length / 64 + 2) * 64];                //если равен или больше 56, то нужен будет еще один блок


            input.CopyTo(input_plus, 0);                                            //копируем входной массив в расширенный
            input_plus[input.Length] = 0x80;                                        //дописываем в конец расширенного еденицу(так как работаем с байтами, записываем 1000 0000 (биты))
            counter = input.Length + 1;                                             //запоминаем текущее положение в массиве
            while (counter % 64 != 56)                                              //пока не дошли до 56 позиции забиваем массив нулями
            {
                input_plus[counter] = 0x00;
                counter++;
            }

            byte[] len_bytes = BitConverter.GetBytes((ulong)input.Length * 8);      //записываем в байтовый массив исходную длину сообшения
            for (int i = input_plus.Length - 8; i < input_plus.Length; i++)         //записываем битовый массив, полученный выше, в конец расширенного массива входных данных
            {
                input_plus[i] = len_bytes[i - (input_plus.Length - 8)];
            }

            uint a = 0x67452301;                                                    //инициализируем начальные значения результирующих переменных
            uint b = 0xefcdab89;
            uint c = 0x98badcfe;
            uint d = 0x10325476;

            for (int num_block = 0; num_block < input_plus.Length / 64; num_block++)//основной цикл для блоков
            {
                block = new uint[16];                                               //инициализируем массив блоков (uint-овый массив, потому что удобно так)
                for (int i = 0; i < 16; i++)                                        //заполняем блоки
                {
                    byte[] temp = new byte[4];
                    temp[0] = input_plus[num_block * 64 + i * 4];                   //берем по четыре байта и запихиваем их в буфер
                    temp[1] = input_plus[num_block * 64 + i * 4 + 1];
                    temp[2] = input_plus[num_block * 64 + i * 4 + 2];
                    temp[3] = input_plus[num_block * 64 + i * 4 + 3];
                    block[i] = BitConverter.ToUInt32(temp, 0);                      //переводим буфер в uint и записываем результат в массив блоков
                }


                uint aa = a, bb = b, cc = c, dd = d;                                //запоминаем значения результирующих переменных до раундов
                a = round(a, b, c, d, 0, 7, 1, 1);                                  //далее 64 раунда
                d = round(d, a, b, c, 1, 12, 2, 1);
                c = round(c, d, a, b, 2, 17, 3, 1);
                b = round(b, c, d, a, 3, 22, 4, 1);

                a = round(a, b, c, d, 4, 7, 5, 1);
                d = round(d, a, b, c, 5, 12, 6, 1);
                c = round(c, d, a, b, 6, 17, 7, 1);
                b = round(b, c, d, a, 7, 22, 8, 1);

                a = round(a, b, c, d, 8, 7, 9, 1);
                d = round(d, a, b, c, 9, 12, 10, 1);
                c = round(c, d, a, b, 10, 17, 11, 1);
                b = round(b, c, d, a, 11, 22, 12, 1);

                a = round(a, b, c, d, 12, 7, 13, 1);
                d = round(d, a, b, c, 13, 12, 14, 1);
                c = round(c, d, a, b, 14, 17, 15, 1);
                b = round(b, c, d, a, 15, 22, 16, 1);

                a = round(a, b, c, d, 1, 5, 17, 2);
                d = round(d, a, b, c, 6, 9, 18, 2);
                c = round(c, d, a, b, 11, 14, 19, 2);
                b = round(b, c, d, a, 0, 20, 20, 2);

                a = round(a, b, c, d, 5, 5, 21, 2);
                d = round(d, a, b, c, 10, 9, 22, 2);
                c = round(c, d, a, b, 15, 14, 23, 2);
                b = round(b, c, d, a, 4, 20, 24, 2);

                a = round(a, b, c, d, 9, 5, 25, 2);
                d = round(d, a, b, c, 14, 9, 26, 2);
                c = round(c, d, a, b, 3, 14, 27, 2);
                b = round(b, c, d, a, 8, 20, 28, 2);

                a = round(a, b, c, d, 13, 5, 29, 2);
                d = round(d, a, b, c, 2, 9, 30, 2);
                c = round(c, d, a, b, 7, 14, 31, 2);
                b = round(b, c, d, a, 12, 20, 32, 2);

                a = round(a, b, c, d, 5, 4, 33, 3);
                d = round(d, a, b, c, 8, 11, 34, 3);
                c = round(c, d, a, b, 11, 16, 35, 3);
                b = round(b, c, d, a, 14, 23, 36, 3);

                a = round(a, b, c, d, 1, 4, 37, 3);
                d = round(d, a, b, c, 4, 11, 38, 3);
                c = round(c, d, a, b, 7, 16, 39, 3);
                b = round(b, c, d, a, 10, 23, 40, 3);

                a = round(a, b, c, d, 13, 4, 41, 3);
                d = round(d, a, b, c, 0, 11, 42, 3);
                c = round(c, d, a, b, 3, 16, 43, 3);
                b = round(b, c, d, a, 6, 23, 44, 3);

                a = round(a, b, c, d, 9, 4, 45, 3);
                d = round(d, a, b, c, 12, 11, 46, 3);
                c = round(c, d, a, b, 15, 16, 47, 3);
                b = round(b, c, d, a, 2, 23, 48, 3);

                a = round(a, b, c, d, 0, 6, 49, 4);
                d = round(d, a, b, c, 7, 10, 50, 4);
                c = round(c, d, a, b, 14, 15, 51, 4);
                b = round(b, c, d, a, 5, 21, 52, 4);

                a = round(a, b, c, d, 12, 6, 53, 4);
                d = round(d, a, b, c, 3, 10, 54, 4);
                c = round(c, d, a, b, 10, 15, 55, 4);
                b = round(b, c, d, a, 1, 21, 56, 4);

                a = round(a, b, c, d, 8, 6, 57, 4);
                d = round(d, a, b, c, 15, 10, 58, 4);
                c = round(c, d, a, b, 6, 15, 59, 4);
                b = round(b, c, d, a, 13, 21, 60, 4);

                a = round(a, b, c, d, 4, 6, 61, 4);
                d = round(d, a, b, c, 11, 10, 62, 4);
                c = round(c, d, a, b, 2, 15, 63, 4);
                b = round(b, c, d, a, 9, 21, 64, 4);

                a += aa; b += bb; c += cc; d += dd;                                 //добавляем результат раундов к результирующим переменным
            }

            byte[] res = new byte[16];

            BitConverter.GetBytes(a).CopyTo(res, 0);                                //запишем битовое представление результирующих переменных в битовый массив res
            BitConverter.GetBytes(b).CopyTo(res, 4);
            BitConverter.GetBytes(c).CopyTo(res, 8);
            BitConverter.GetBytes(d).CopyTo(res, 12);

            StringBuilder sb = new StringBuilder();                                 //переведем res в строку hex-а
            for (int i = 0; i < res.Length; i++)
            {
                sb.Append(res[i].ToString("X2"));
            }
            return sb.ToString();
        }

        //раундовые функции
        uint F(uint x, uint y, uint z) { return (x & y) | (~x & z); }
        uint G(uint x, uint y, uint z) { return (x & z) | (~z & y); }
        uint H(uint x, uint y, uint z) { return x ^ y ^ z; }
        uint I(uint x, uint y, uint z) { return y ^ (~z | x); }

        //метод для 64 раундов
        uint round(uint a, uint b, uint c, uint d, uint k, int s, uint i, uint func_num)
        {
            // вазвисимости от раунда выбираем одну из раундовых функций(с 1 по 16 раунды функция F)
            if (func_num == 1)
                a += F(b, c, d) + block[k] + K[i];
            if (func_num == 2)
                a += G(b, c, d) + block[k] + K[i];
            if (func_num == 3)
                a += H(b, c, d) + block[k] + K[i];
            if (func_num == 4)
                a += I(b, c, d) + block[k] + K[i];
            a = (a << s | a >> (32 - s));   //циклический битовый сдвиг влево на s
            a += b;

            return a;
        }

    }
}

