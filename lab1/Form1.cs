using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        private const string inputFile = "Входной файл...", outputFile = "Каталог сохранения выходных файлов...";
        private string inputFileName, outputDirectory;
        private byte[,] SBoxE = {
            { 0x63,0x7c,0x77,0x7b,0xf2,0x6b,0x6f,0xc5,0x30,0x01,0x67,0x2b,0xfe,0xd7,0xab,0x76 },
            { 0xca,0x82,0xc9,0x7d,0xfa,0x59,0x47,0xf0,0xad,0xd4,0xa2,0xaf,0x9c,0xa4,0x72,0xc0 },
            { 0xb7,0xfd,0x93,0x26,0x36,0x3f,0xf7,0xcc,0x34,0xa5,0xe5,0xf1,0x71,0xd8,0x31,0x15 },
            { 0x04,0xc7,0x23,0xc3,0x18,0x96,0x05,0x9a,0x07,0x12,0x80,0xe2,0xeb,0x27,0xb2,0x75 },
            { 0x09,0x83,0x2c,0x1a,0x1b,0x6e,0x5a,0xa0,0x52,0x3b,0xd6,0xb3,0x29,0xe3,0x2f,0x84 },
            { 0x53,0xd1,0x00,0xed,0x20,0xfc,0xb1,0x5b,0x6a,0xcb,0xbe,0x39,0x4a,0x4c,0x58,0xcf },
            { 0xd0,0xef,0xaa,0xfb,0x43,0x4d,0x33,0x85,0x45,0xf9,0x02,0x7f,0x50,0x3c,0x9f,0xa8 },
            { 0x51,0xa3,0x40,0x8f,0x92,0x9d,0x38,0xf5,0xbc,0xb6,0xda,0x21,0x10,0xff,0xf3,0xd2 },
            { 0xcd,0x0c,0x13,0xec,0x5f,0x97,0x44,0x17,0xc4,0xa7,0x7e,0x3d,0x64,0x5d,0x19,0x73 },
            { 0x60,0x81,0x4f,0xdc,0x22,0x2a,0x90,0x88,0x46,0xee,0xb8,0x14,0xde,0x5e,0x0b,0xdb },
            { 0xe0,0x32,0x3a,0x0a,0x49,0x06,0x24,0x5c,0xc2,0xd3,0xac,0x62,0x91,0x95,0xe4,0x79 },
            { 0xe7,0xc8,0x37,0x6d,0x8d,0xd5,0x4e,0xa9,0x6c,0x56,0xf4,0xea,0x65,0x7a,0xae,0x08 },
            { 0xba,0x78,0x25,0x2e,0x1c,0xa6,0xb4,0xc6,0xe8,0xdd,0x74,0x1f,0x4b,0xbd,0x8b,0x8a },
            { 0x70,0x3e,0xb5,0x66,0x48,0x03,0xf6,0x0e,0x61,0x35,0x57,0xb9,0x86,0xc1,0x1d,0x9e },
            { 0xe1,0xf8,0x98,0x11,0x69,0xd9,0x8e,0x94,0x9b,0x1e,0x87,0xe9,0xce,0x55,0x28,0xdf },
            { 0x8c,0xa1,0x89,0x0d,0xbf,0xe6,0x42,0x68,0x41,0x99,0x2d,0x0f,0xb0,0x54,0xbb,0x16 }
        };
        private byte[,] RCon =
        {
            { 0x1,0x2,0x4,0x8, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36},
            { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 },
            { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 },
            { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 },
        };
        private byte[,] SBoxD ={
            { 0x52,0x09,0x6a,0xd5,0x30,0x36,0xa5,0x38,0xbf,0x40,0xa3,0x9e,0x81,0xf3,0xd7,0xfb },
            { 0x7c,0xe3,0x39,0x82,0x9b,0x2f,0xff,0x87,0x34,0x8e,0x43,0x44,0xc4,0xde,0xe9,0xcb },
            { 0x54,0x7b,0x94,0x32,0xa6,0xc2,0x23,0x3d,0xee,0x4c,0x95,0x0b,0x42,0xfa,0xc3,0x4e },
            { 0x08,0x2e,0xa1,0x66,0x28,0xd9,0x24,0xb2,0x76,0x5b,0xa2,0x49,0x6d,0x8b,0xd1,0x25 },
            { 0x72,0xf8,0xf6,0x64,0x86,0x68,0x98,0x16,0xd4,0xa4,0x5c,0xcc,0x5d,0x65,0xb6,0x92 },
            { 0x6c,0x70,0x48,0x50,0xfd,0xed,0xb9,0xda,0x5e,0x15,0x46,0x57,0xa7,0x8d,0x9d,0x84 },
            { 0x90,0xd8,0xab,0x00,0x8c,0xbc,0xd3,0x0a,0xf7,0xe4,0x58,0x05,0xb8,0xb3,0x45,0x06 },
            { 0xd0,0x2c,0x1e,0x8f,0xca,0x3f,0x0f,0x02,0xc1,0xaf,0xbd,0x03,0x01,0x13,0x8a,0x6b },
            { 0x3a,0x91,0x11,0x41,0x4f,0x67,0xdc,0xea,0x97,0xf2,0xcf,0xce,0xf0,0xb4,0xe6,0x73 },
            { 0x96,0xac,0x74,0x22,0xe7,0xad,0x35,0x85,0xe2,0xf9,0x37,0xe8,0x1c,0x75,0xdf,0x6e },
            { 0x47,0xf1,0x1a,0x71,0x1d,0x29,0xc5,0x89,0x6f,0xb7,0x62,0x0e,0xaa,0x18,0xbe,0x1b },
            { 0xfc,0x56,0x3e,0x4b,0xc6,0xd2,0x79,0x20,0x9a,0xdb,0xc0,0xfe,0x78,0xcd,0x5a,0xf4 },
            { 0x1f,0xdd,0xa8,0x33,0x88,0x07,0xc7,0x31,0xb1,0x12,0x10,0x59,0x27,0x80,0xec,0x5f },
            { 0x60,0x51,0x7f,0xa9,0x19,0xb5,0x4a,0x0d,0x2d,0xe5,0x7a,0x9f,0x93,0xc9,0x9c,0xef },
            { 0xa0,0xe0,0x3b,0x4d,0xae,0x2a,0xf5,0xb0,0xc8,0xeb,0xbb,0x3c,0x83,0x53,0x99,0x61 },
            { 0x17,0x2b,0x04,0x7e,0xba,0x77,0xd6,0x26,0xe1,0x69,0x14,0x63,0x55,0x21,0x0c,0x7d }
        };

        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            radioButton3.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            OPF.Filter = "Файлы |*";
            OPF.Title = "Выбрать файл";
            if (OPF.ShowDialog() == DialogResult.OK)
            {
                inputFileName = OPF.FileName;
                label1.Text = inputFileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                outputDirectory = FBD.SelectedPath;
                FBD.Description = "Выбрать директорию";
                label2.Text = outputDirectory;
            }
        }

        private void Finish()
        {
            label1.Text = inputFile;
            label2.Text = outputFile;
            textBox1.Text = "";
            MessageBox.Show("Успех!");
        }

        // операция XOR ключа и блока
        private byte[,] AddRoundKey(byte[,] key, byte[,] State)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    State[i, j] ^= key[i, j];
                }
            }
            return State;
        }

        //операция перемешивания байт в блоке для шифрования
        private byte[,] SubBytes(byte[,] State)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    byte value = State[i, j];
                    // вычлениям левый полубайт
                    byte valueRow = (byte)(value >> 4);
                    //вычленяем правый полубайт
                    byte valueColumn = (byte)(value << 4);
                    valueColumn >>= 4;
                    State[i, j] = SBoxE[valueRow, valueColumn];
                }
            }
            return State;
        }

        //операция перемешивания байт в блоке для дешифрования
        private byte[,] InvSubBytes(byte[,] State)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    byte value = State[i, j];
                    // вычлениям левый полубайт
                    byte valueRow = (byte)(value >> 4);
                    //вычленяем правый полубайт
                    byte valueColumn = (byte)(value << 4);
                    valueColumn >>= 4;
                    State[i, j] = SBoxD[valueRow, valueColumn];
                }
            }
            return State;
        }

        // операция сдвига строк для шифрования -  циклический сдвиг влево на 1 элемент для первой строки, 
        //на 2 для второй и на 3 для третьей. Нулевая строка не сдвигается. 
        private byte[,] ShiftRows(byte[,] State)
        {
            for (int i = 1; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        {
                            byte temp1 = State[i, 0];
                            State[i, 0] = State[i, 1];
                            State[i, 1] = State[i, 2];
                            State[i, 2] = State[i, 3];
                            State[i, 3] = temp1;
                            break;
                        }
                    case 2:
                        {
                            byte temp1 = State[i, 0];
                            byte temp2 = State[i, 1];
                            State[i, 0] = State[i, 2];
                            State[i, 1] = State[i, 3];
                            State[i, 3] = temp2;
                            State[i, 2] = temp1;
                            break;
                        }
                    case 3:
                        {
                            byte temp1 = State[i, 0];
                            byte temp2 = State[i, 1];
                            byte temp3 = State[i, 2];
                            State[i, 0] = State[i, 3];
                            State[i, 1] = temp1;
                            State[i, 2] = temp2;
                            State[i, 3] = temp3;
                            break;
                        }
                }
            }
            return State;
        }

        //операция сдвига строк для дешифрования
        private byte[,] InvShiftRows(byte[,] State)
        {
            for (int i = 1; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        {
                            byte temp1 = State[i, 3];
                            State[i, 3] = State[i, 2];
                            State[i, 2] = State[i, 1];
                            State[i, 1] = State[i, 0];
                            State[i, 0] = temp1;
                            break;
                        }
                    case 2:
                        {
                            byte temp1 = State[i, 3];
                            byte temp2 = State[i, 2];
                            State[i, 3] = State[i, 1];
                            State[i, 2] = State[i, 0];
                            State[i, 1] = temp1;
                            State[i, 0] = temp2;
                            break;
                        }
                    case 3:
                        {
                            byte temp1 = State[i, 3];
                            byte temp2 = State[i, 2];
                            byte temp3 = State[i, 1];
                            State[i, 3] = State[i, 0];
                            State[i, 0] = temp3;
                            State[i, 1] = temp2;
                            State[i, 2] = temp1;
                            break;
                        }
                }
            }
            return State;
        }

        //вспомогательные функции умножения для операций MixColumns и InvMixColumns
        private byte mul_by_02(byte num)
        {
            byte res;
            if (num < 0x80)
                res = (byte)(num << 1);
            else
                res = (byte)((num << 1) ^ 0x1b);
            return (byte)(res % 0x100);
        }

        private byte mul_by_03(byte num)
        {
            return (byte)(mul_by_02(num) ^ num);
        }

        private byte mul_by_09(byte num)
        {
            return (byte)(mul_by_02(mul_by_02(mul_by_02(num))) ^ num);
        }

        private byte mul_by_0b(byte num)
        {
            return (byte)(mul_by_02(mul_by_02(mul_by_02(num))) ^ mul_by_02(num) ^ num);
        }

        private byte mul_by_0d(byte num)
        {
            return (byte)(mul_by_02(mul_by_02(mul_by_02(num))) ^ mul_by_02(mul_by_02(num)) ^ num);
        }

        private byte mul_by_0e(byte num)
        {
            return (byte)(mul_by_02(mul_by_02(mul_by_02(num))) ^ mul_by_02(mul_by_02(num)) ^ mul_by_02(num));
        }
        //--------------------------конец функций умножения ---------------------------------------------------------

        // операция перемешивания колон для дешифрования
        private byte[,] InvMixColumns(byte[,] State)
        {
            //массив из 4х строк для столбца State
            byte[,] temp = new byte[4, 1];
            for (int j = 0; j < 4; j++)
            {
                //вычленяем столбец
                for (int i = 0; i < 4; i++)
                {
                    temp[i, 0] = State[i, j];
                }
                //создаём результирующий массив
                byte[,] result = new byte[4, 1];
                result[0, 0] = (byte)((mul_by_0e(temp[0, 0])) ^ (mul_by_0b(temp[1, 0])) ^ (mul_by_0d(temp[2, 0])) ^ (mul_by_09(temp[3, 0])));
                result[1, 0] = (byte)((mul_by_09(temp[0, 0])) ^ (mul_by_0e(temp[1, 0])) ^ (mul_by_0b(temp[2, 0])) ^ (mul_by_0d(temp[3, 0])));
                result[2, 0] = (byte)((mul_by_0d(temp[0, 0])) ^ (mul_by_09(temp[1, 0])) ^ (mul_by_0e(temp[2, 0])) ^ (mul_by_0b(temp[3, 0])));
                result[3, 0] = (byte)((mul_by_0b(temp[0, 0])) ^ (mul_by_0d(temp[1, 0])) ^ (mul_by_09(temp[2, 0])) ^ (mul_by_0e(temp[3, 0])));
                //записываем его в соответствующую колонку матрицы
                for (int i = 0; i < 4; i++)
                {
                    State[i, j] = result[i, 0];
                }
            }
            return State;
        }

        // операция перемешивания колон для шифрования
        private byte[,] MixColumns(byte[,] State)
        {
            //массив из 4х строк для столбца State
            byte[,] temp = new byte[4, 1];
            for (int j = 0; j < 4; j++)
            {
                //вычленяем столбец
                for (int i = 0; i < 4; i++)
                {
                    temp[i, 0] = State[i, j];
                }
                //создаём результирующий массив
                byte[,] result = new byte[4, 1];
                result[0, 0] = (byte)((mul_by_02(temp[0, 0])) ^ (mul_by_03(temp[1, 0])) ^ temp[2, 0] ^ temp[3, 0]);
                result[1, 0] = (byte)(temp[0, 0] ^ (mul_by_02(temp[1, 0])) ^ (mul_by_03(temp[2, 0])) ^ temp[3, 0]);
                result[2, 0] = (byte)(temp[0, 0] ^ temp[1, 0] ^ (mul_by_02(temp[2, 0])) ^ (mul_by_03(temp[3, 0])));
                result[3, 0] = (byte)((mul_by_03(temp[0, 0])) ^ temp[1, 0] ^ temp[2, 0] ^ (mul_by_02(temp[3, 0])));
                //записываем его в соответствующую колонку матрицы
                for (int i = 0; i<4; i++)
                {
                    State[i,j] = result[i,0];
                }
            }
            return State;
        }

        //фнуцкия получения ключей для каждого раунда
        private byte[,] KeyExpansion(byte[] key)
        {
            byte[,] keys = new byte[4, 44];
            // заполняем первый ключ (исходный)
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    keys[i, j] = key[i + j * 4];
                }
            }
            //вычисляем остальные ключи
            for(int i=4; i<44; i++)
            {
                if(i%4 == 0) // если номер колонки кратен 4
                {
                    // вычленяем колонку i-1 в матрице
                    byte[,] w_dec_i = new byte[4, 1];
                    for(int e = 0; e<4; e++)
                    {
                        w_dec_i[e, 0] = keys[e, i-1];
                    }
                    //сдвигаем циклически на 1 вверх
                    byte temp1 = w_dec_i[0, 0];
                    w_dec_i[0, 0] = w_dec_i[3, 0];
                    w_dec_i[1, 0] = w_dec_i[2, 0];
                    w_dec_i[2, 0] = w_dec_i[3, 0];
                    w_dec_i[3, 0] = temp1;
                    //заменяем байты по таблице SBOx
                    for (int j = 0; j < 4; j++)
                    {
                        byte value = w_dec_i[j, 0];
                        // вычлениям левый полубайт
                        byte valueRow = (byte)(value >> 4);
                        //вычленяем правый полубайт
                        byte valueColumn = (byte)(value << 4);
                        valueColumn >>= 4;
                        w_dec_i[j, 0] = SBoxE[valueRow, valueColumn];
                    }
                    //выполняем операцию XOR между колонкой Wi-4, измененной Wi-1 и колонкой Rconi/4. Результат записывается в колонку Wi
                    for(int j=0; j<4; j++)
                    {
                        w_dec_i[j, 0] = (byte)(w_dec_i[j, 0] ^ RCon[j, (i / 4) - 1] ^ keys[j, i - 4]);
                    }
                    //записываем результат в массив ключей
                    for (int e = 0; e < 4; e++)
                    {
                        keys[e, i] = w_dec_i[e,0];
                    }
                }
                else  // если номер колонки не кратен 4
                {
                    for (int e = 0; e < 4; e++)
                    {
                        keys[e, i] = (byte)(keys[e, i-4] ^ keys[e, i - 1]);
                    }
                }
            }
            return keys;
        }

        //функция получения расширения файла
        private string GetExtension(string name)
        {
            int i;
            for (i = name.Length - 1; i>=0; i--)
            {
                if (name[i] == '.')
                    break;
            }
            string extension="";
            for(int j=i+1; j<name.Length; j++)
            {
                extension += name[j];
            }
            return extension;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((label1.Text == inputFile) || (label2.Text == outputFile))  //проверка выбора файлов
            {
                MessageBox.Show("Не выбраны пути к файлам!");
            }
            else
            {
                if(textBox1.Text == "")
                {
                    MessageBox.Show("Ошибка! Ключ не может быть пустым!");
                    return;
                }
                string keyString = textBox1.Text; // считывание ключа из textbox в строку
                byte[] key = new byte[16]; //работаем с байтами => надо преобразовать ключ-стрроку в массив байтов (размер равен 128 бит или 16 байт)
                if (radioButton2.Checked) // если ключ - 16ричное число
                {
                    string[] hexValuesSplit = keyString.Split(' '); // разбваем строку на группы чисел
                    if (hexValuesSplit.Length != (16)) // если групп не 16, значит строка неверной длины
                    {
                        MessageBox.Show("Ошибка! Неверный формат строки!");
                        return;
                    }
                    else
                    {
                        int i = 0;
                        foreach (string hex in hexValuesSplit) // рассматриваем каждую группу 
                        {
                            byte value;
                            try
                            {
                                value = Convert.ToByte(hex, 16); // пытаемся преобразовать группу числа в 16ричное число
                            }
                            catch
                            {
                                MessageBox.Show("Ошибка! присутствуют недопустимые символы!");
                                return;
                            }
                            key[i] = value;
                            i++;
                        }
                    }
                }
                else // если ключ - текст
                {
                    // получение хэша md5 (он как раз равен 128 байт) 
                    byte[] tmpSource;
                    tmpSource = ASCIIEncoding.ASCII.GetBytes(keyString);
                    key = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
                }
                if (radioButton3.Checked) // если шифрование
                {
                    //открываем входной файл
                    FileStream fstream = new FileStream(inputFileName, FileMode.Open);
                    byte[] file;
                    byte[] beforeCoding = new byte[1]; // хранит количество байт, занятых в последнем блоке
                    if (fstream.Length % 16 == 0)
                    {
                        file = new byte[fstream.Length];
                        beforeCoding[0] = 16;
                    }
                    else
                    {
                        file = new byte[fstream.Length + (16 - fstream.Length % 16)]; // если не кратно 16, то создаем массив кратный 16
                        beforeCoding[0] = (byte)(fstream.Length % 16);
                    }
                    // считываем файл в массив
                    fstream.Read(file, 0, file.Length);
                    //создаём выходной файл
                    string path = outputDirectory + "\\ЗАШИФРОВАННЫЙ ФАЙЛ";
                    FileStream foutstream = new FileStream(path, FileMode.Create);
                    //записываем в него в качестве первого байта количество использованный байт в последнем блоке
                    foutstream.Write(beforeCoding, 0, beforeCoding.Length);
                    // получаем расширение входного файла
                    string extension = GetExtension(inputFileName);
                    beforeCoding[0] = (byte)extension.Length; // хранит количество символов в расширении файл
                    if(extension.Length != inputFileName.Length) // если расширение не равно длине файла
                    {
                        foutstream.Write(beforeCoding, 0, beforeCoding.Length); // записываем в файл в качестве второго байта количество символов в расширении файла
                    }
                    else // иначе это значит, что расширения нет => записываем 0
                    {
                        beforeCoding[0] = 0;
                        foutstream.Write(beforeCoding, 0, beforeCoding.Length); // записываем в файл в качестве второго байта количество символов в расширении файла
                    }
                    // записываем в файл символы расширения файла
                    for (int i=0; i < beforeCoding[0]; i++)
                    {
                        byte[] array = new byte[1];
                        array[0] = (byte)extension[i];
                        foutstream.Write(array, 0, array.Length); 
                    }
                    byte[,] RoundKey = new byte[4, 4]; // представляем ключ в виде матрицы 4*4
                    // записываем ключ по столбцам сверху вниз
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            RoundKey[i, j] = key[i + j * 4];
                        }
                    }
                    //проходимся по всему файлу, разделяя его на блоки по 16 байт
                    for (int i = 0; i < (file.Length / 16); i++)
                    {
                        // представляем блок в виде матрицы 4*4
                        byte[,] State = new byte[4, 4];
                        for (int j = 0; j < 4; j++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                State[k, j] = file[k + j * 4 + i * 16];
                            }
                        }
                        //формируем ключи
                        byte[,] keys = new byte[4, 11];
                        keys = KeyExpansion(key);
                        // инициалиирующий раунд
                        for (int w = 0; w < 4; w++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                RoundKey[x, w] = keys[x, w];
                            }
                        }
                        State = AddRoundKey(RoundKey, State);
                        // для aes-128 9 раундов шифрования
                        for (int r = 1; r < 10; r++)
                        {
                            State = SubBytes(State);
                            State = ShiftRows(State);
                            State = MixColumns(State);
                            //изменяем ключ
                            for (int w = 0; w<4; w++)
                            {
                                for(int x=0; x<4; x++)
                                {
                                    RoundKey[x,w] = keys[x,w+r*4];
                                }
                            }
                            //применяем новый ключ
                            State = AddRoundKey(RoundKey, State);
                        }
                        // последний раунд
                        State = SubBytes(State);
                        State = ShiftRows(State);
                        // берём последний ключ
                        for (int w = 0; w < 4; w++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                RoundKey[x, w] = keys[x, w + 40];
                            }
                        }
                        State = AddRoundKey(RoundKey, State);
                        //представляем блок в виде одномерного массива
                        byte[] outputMas = new byte[16];
                        for (int f = 0; f < 4; f++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                outputMas[4*f + j ] = State[j, f];
                            }
                        }
                        //записываем его в файл
                        foutstream.Write(outputMas, 0, outputMas.Length);
                    }
                    //закрываем файлы
                    fstream.Close();
                    foutstream.Close();
                }
                else // если дешифрование
                {
                    if (textBox1.Text == "")
                    {
                        MessageBox.Show("Ошибка! Ключ не может быть пустым!");
                        return;
                    }
                    //открываем файл
                    FileStream fstream = new FileStream(inputFileName, FileMode.Open);
                    byte[] file = new byte[fstream.Length];
                    // считываем файл в массив
                    fstream.Read(file, 0, file.Length);
                    byte lastByteCount = file[0]; // хранит количество байтов использованных в последнем блоке
                    //создаём выходной файл
                    byte extensionCount = file[1]; // хранит количество символов в расширении файла
                    //byte[] extensionArray = new byte[extensionCount];
                    string path = outputDirectory + "\\РАСШИФРОВАННЫЙ ФАЙЛ.";
                    //создаём полный путь к выходному файлу
                    for (int i = 2; i < extensionCount+2; i++)
                    {
                        path += (char)file[i];
                    }
                    // открываем файл
                    FileStream foutstream = new FileStream(path, FileMode.Create);
                    byte[,] RoundKey = new byte[4, 4]; // представляем ключ в виде матрицы 4*4
                    // записываем ключ по столбцам сверху вниз
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            RoundKey[i, j] = key[i + j * 4];
                        }
                    }
                    //проходимся по всему файлу, разделяя его на блоки по 16 байт
                    for (int i = 0; i < ((file.Length - extensionCount - 2 )/ 16); i++)
                    {
                        // представляем блок в виде матрицы 4*4
                        byte[,] State = new byte[4, 4];
                        for (int j = 0; j < 4; j++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                State[k, j] = file[k + j * 4 + i * 16 + extensionCount + 2];
                            }
                        }
                        //формируем ключи
                        byte[,] keys = new byte[4, 11];
                        keys = KeyExpansion(key);
                        // начальный ключ выбирается с конца
                        for (int w = 0; w < 4; w++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                RoundKey[x, w] = keys[x, w+40];
                            }
                        }
                        // инициалиирующий раунд
                        State = AddRoundKey(RoundKey, State);
                        // для aes-128 9 раундов дешифрования
                        for (int r = 9; r > 0; r--)
                        {
                            State = InvShiftRows(State);
                            State = InvSubBytes(State);
                            //изменяем ключ
                            for (int w = 0; w < 4; w++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    RoundKey[x, w] = keys[x, w + r * 4];
                                }
                            }
                            State = AddRoundKey(RoundKey, State);
                            State = InvMixColumns(State);
                        }
                        // последний раунд
                        State = InvSubBytes(State);
                        State = InvShiftRows(State);
                        // берём самый первый ключ
                        for (int w = 0; w < 4; w++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                RoundKey[x, w] = keys[x, w];
                            }
                        }
                        State = AddRoundKey(RoundKey, State);
                        //представляем блок в виде одномерного массива
                        byte[] outputMas = new byte[16];
                        for (int f = 0; f < 4; f++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                outputMas[4*f + j] = State[j, f];
                            }
                        }
                        //записываем его в файл
                        if(i == (file.Length / 16)-1) // если последний блок, то записываем только нужное число блоков 
                        {
                            foutstream.Write(outputMas, 0, lastByteCount);
                        }
                        else // иначе записываем весь блок
                        {
                            foutstream.Write(outputMas, 0, outputMas.Length);
                        }
                    }
                    //закрываем файлы
                    fstream.Close();
                    foutstream.Close();
                }
                Finish();
            }
        }
    }
}
