using System;
using CPFT.CremulParser;
using CPFT.CremulParser.Cremul;
using NUnit.Framework;

namespace Tests
{
    public class Parser
    {

        private string ResolveFilePath(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            return path + "\\..\\..\\Files\\" + fileName;
        }

        [Test]
        public void ValidCremulWithOneLine()
        {
            var parser = new CremulParser();
            parser.Parse(ResolveFilePath("CREMUL0001.txt"));

            Assert.AreEqual(1, parser.Messages.Count);

            var d20140312 = new DateTime(2014, 3, 12);

            var msg = parser.Messages[0];
            Assert.AreEqual(1, msg.MessageIndex);
            Assert.IsTrue(msg.Header.MsgId.Contains("CREMUL"));
            Assert.AreEqual(d20140312, msg.Header.CreatedDate);
            Assert.AreEqual(975945065, msg.Header.BfId);
            Assert.AreEqual(1, msg.NumberOfLines);

            var line = msg.Lines[0];
            Assert.AreEqual(1, line.LineIndex);
            Assert.AreEqual(d20140312, line.PostingDate);
            Assert.AreEqual("12121212121", line.BfAccountNumber);

            Assert.AreEqual(1394.0, line.Money.Amount);
            Assert.AreEqual("NOK", line.Money.Currency);
            var lref = line.Reference;
            Assert.AreEqual("ACK", lref.Type);
            Assert.AreEqual("08012992096", lref.Number);

            Assert.AreEqual(1, line.Transactions.Count);

            PaymentTx tx = line.Transactions[0];
            Assert.AreEqual(1, tx.TxIndex);
            Assert.AreEqual(d20140312, tx.PostingDate);
            Assert.AreEqual(1394.0, tx.Money.Amount);
            Assert.AreEqual("NOK", tx.Money.Currency);
            Assert.AreEqual("12312312312", tx.PayerAccountNumber);
            Assert.AreEqual(2, tx.References.Count);
            Assert.AreEqual("Tømrer Morten Rognebær AS", tx.FreeText);

            var txref1 = tx.References[0];
            Assert.AreEqual("AEK", txref1.Type);
            Assert.AreEqual("12072200001", txref1.Number);

            var txref2 = tx.References[1];
            Assert.AreEqual("ACD", txref2.Type);
            Assert.AreEqual("180229451", txref2.Number);

            var txPayer = tx.PayerNad;
            Assert.AreEqual("PL", txPayer.Type);
            Assert.AreEqual("Skjerpåkeren 17", txPayer.NadLines[1]);
        }


        [Test]
        public void ValidCremulWithMultiLines()
        {
            var parser = new CremulParser();
            parser.Parse(ResolveFilePath("cremul_multi_lines.txt"));

            var d20110111 = new DateTime(2011, 1, 11);

            var msg = parser.Messages[0];
            Assert.IsTrue(msg.Header.MsgId.Contains("CREMUL"));
            Assert.AreEqual(d20110111, msg.Header.CreatedDate);

            Assert.AreEqual(3, msg.NumberOfLines);

            var line1 = msg.Lines[0];
            Assert.AreEqual(d20110111, line1.PostingDate);
            Assert.AreEqual(14637.0, line1.Money.Amount);
            Assert.AreEqual("NOK", line1.Money.Currency);

            Assert.AreEqual(1, line1.Transactions.Count);
            var tx = line1.Transactions[0];
            Assert.AreEqual(d20110111, tx.PostingDate);
            Assert.AreEqual(14637, tx.Money.Amount);
            Assert.AreEqual("NOK", tx.Money.Currency);
            Assert.AreEqual(2, tx.References.Count);

            Assert.AreEqual("PL", tx.PayerNad.Type);
            Assert.AreEqual("ØKONOMIKONTORET 5 ETG", tx.PayerNad.NadLines[1]);


            var line2 = msg.Lines[1];
            Assert.AreEqual(d20110111, line2.PostingDate);
            Assert.AreEqual(15000.0, line2.Money.Amount);
            Assert.AreEqual("NOK", line2.Money.Currency);
            Assert.AreEqual(1, line2.Transactions.Count);

            var line3 = msg.Lines[2];
            Assert.AreEqual(d20110111, line3.PostingDate);
            Assert.AreEqual(6740.40, line3.Money.Amount);
            Assert.AreEqual("NOK", line3.Money.Currency);
            Assert.AreEqual(2, line3.Transactions.Count);
        }

        [Test]
        public void ValidCremulLongFile()
        {
            var parser = new CremulParser();
            parser.Parse(ResolveFilePath("CREMUL0003.txt"));

            var d20130411 = new DateTime(2013, 4, 11);

            var msg = @parser.Messages[0];
            Assert.IsTrue(msg.Header.MsgId.Contains("CREMUL"));
            Assert.AreEqual(d20130411, msg.Header.CreatedDate);

            Assert.AreEqual(4, msg.NumberOfLines);

            var line1 = msg.Lines[0];
            Assert.AreEqual(d20130411, line1.PostingDate);
            Assert.AreEqual(3000.0, line1.Money.Amount);
            Assert.AreEqual("NOK", line1.Money.Currency);

            Assert.AreEqual(12, line1.Transactions.Count);

            var tx = line1.Transactions[0];
            Assert.AreEqual(new DateTime(2013, 4, 10), tx.PostingDate);
            Assert.AreEqual(250.0, tx.Money.Amount);
            Assert.AreEqual("NOK", tx.Money.Currency);
            Assert.AreEqual("12345678901", tx.PayerAccountNumber);
            Assert.AreEqual("20132065978", tx.InvoiceRef);
            Assert.AreEqual(RefTypes.Kid, tx.InvoiceRefType);
        }

        [Test]
        public void ValidCremulAddresParse()
        {
            var parser = new CremulParser();
            parser.Parse(ResolveFilePath("CREMUL0002.DAT"));

            var msg = @parser.Messages[0];

            var tx = msg.Lines[0].Transactions[0];
            var txPayer = tx.PayerNad;
            Assert.AreEqual("PL",txPayer.Type);
            Assert.AreEqual("Ole Thomessen", txPayer.NadLines[0]);
            Assert.AreEqual("St. Nikolas-Gate 7", txPayer.NadLines[1]);
            Assert.AreEqual("1706 SARPSBORG", txPayer.NadLines[3]);
        }

        [Test]
        public void ValidCremulWithEmptyFIIElems()
        {
            var parser = new CremulParser();
            parser.Parse(ResolveFilePath("CREMUL0001.DAT"));

            var msg = parser.Messages[0];
             Assert.IsTrue(msg.Header.MsgId.Contains("CREMUL"));

             Assert.IsNull(msg.Lines[0].Transactions[0].PayerAccountNumber);     
        }

        [Test, Ignore]
        public void ValidCremulMultiMessageFile()
        {

        }
    }
}
