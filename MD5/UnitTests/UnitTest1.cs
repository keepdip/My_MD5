using Xunit;


namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string input = "md5";
            MD5.Crypt crypt = new MD5.Crypt();

            string res = crypt.my_md5(input);

            Assert.Equal(crypt.etalon_md5(input), res);
        }

        [Fact]
        public void Test2()
        {
            string input = "Hello";
            MD5.Crypt crypt = new MD5.Crypt();

            string res = crypt.my_md5(input);

            Assert.Equal(crypt.etalon_md5(input), res);
        }

        [Fact]
        public void Test3()
        {
            string input = "ejkehfkjahskfjashoqwhurhquwruholbkjdbskgvbbvsdkbbdjquwy";   //55 символов
            MD5.Crypt crypt = new MD5.Crypt();

            string res = crypt.my_md5(input);

            Assert.Equal(crypt.etalon_md5(input), res);
        }

        [Fact]
        public void Test4()
        {
            string input = "ejkehfkjahskfjashoqwhurhquwruholbkjdbskgvbbvsdkbbdjquwqy";   //56 символов
            MD5.Crypt crypt = new MD5.Crypt();

            string res = crypt.my_md5(input);

            Assert.Equal(crypt.etalon_md5(input), res);
        }

        [Fact]
        public void Test5()
        {
            string input = "ejkehfkjahskfjashoqwhurhquwruholbkjdbskgvbbv2sdkbbdjquwqy";   //57 символов
            MD5.Crypt crypt = new MD5.Crypt();

            string res = crypt.my_md5(input);

            Assert.Equal(crypt.etalon_md5(input), res);
        }

    }
}
