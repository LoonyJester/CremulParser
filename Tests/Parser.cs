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
            parser.parse(ResolveFilePath("CREMUL0001.txt"));

            Assert.AreEqual(1, parser.messages.Count);

            var d20140312 = new DateTime(2014, 3, 12);

            var msg = parser.messages[0];
            Assert.AreEqual(1, msg.message_index);
            Assert.IsTrue(msg.header.msg_id.Contains("CREMUL"));
            Assert.AreEqual(d20140312, msg.header.created_date);
            Assert.AreEqual(975945065, msg.header.bf_id);
            Assert.AreEqual(1, msg.number_of_lines);

            var line = msg.lines[0];
            Assert.AreEqual(1, line.line_index);
            Assert.AreEqual(d20140312, line.posting_date);
            Assert.AreEqual("12121212121", line.bf_account_number);

            Assert.AreEqual(1394.0, line.money.amount);
            Assert.AreEqual("NOK", line.money.currency);
            var lref = line.reference;
            Assert.AreEqual("ACK", lref.type);
            Assert.AreEqual("08012992096", lref.number);

            Assert.AreEqual(1, line.transactions.Count);

            PaymentTx tx = line.transactions[0];
            Assert.AreEqual(1, tx.tx_index);
            Assert.AreEqual(d20140312, tx.posting_date);
            Assert.AreEqual(1394.0, tx.money.amount);
            Assert.AreEqual("NOK", tx.money.currency);
            Assert.AreEqual("12312312312", tx.payer_account_number);
            Assert.AreEqual(2, tx.references.Count);
            Assert.AreEqual("Tømrer Morten Rognebær AS", tx.free_text);

            var txref1 = tx.references[0];
            Assert.AreEqual("AEK", txref1.type);
            Assert.AreEqual("12072200001", txref1.number);

            var txref2 = tx.references[1];
            Assert.AreEqual("ACD", txref2.type);
            Assert.AreEqual("180229451", txref2.number);

            var txPayer = tx.payer_nad;
            Assert.AreEqual("PL", txPayer.type);
            Assert.AreEqual("Skjerpåkeren 17", txPayer.nad_lines[1]);
        }


        [Test]
        public void ValidCremulWithMultiLines()
        {
            var parser = new CremulParser();
            parser.parse(ResolveFilePath("cremul_multi_lines.txt"));

            var d20110111 = new DateTime(2011, 1, 11);

            var msg = parser.messages[0];
            Assert.IsTrue(msg.header.msg_id.Contains("CREMUL"));
            Assert.AreEqual(d20110111, msg.header.created_date);

            Assert.AreEqual(3, msg.number_of_lines);

            var line1 = msg.lines[0];
            Assert.AreEqual(d20110111, line1.posting_date);
            Assert.AreEqual(14637.0, line1.money.amount);
            Assert.AreEqual("NOK", line1.money.currency);

            Assert.AreEqual(1, line1.transactions.Count);
            var tx = line1.transactions[0];
            Assert.AreEqual(d20110111, tx.posting_date);
            Assert.AreEqual(14637, tx.money.amount);
            Assert.AreEqual("NOK", tx.money.currency);
            Assert.AreEqual(2, tx.references.Count);

            Assert.AreEqual("PL", tx.payer_nad.type);
            Assert.AreEqual("ØKONOMIKONTORET 5 ETG", tx.payer_nad.nad_lines[1]);


            var line2 = msg.lines[1];
            Assert.AreEqual(d20110111, line2.posting_date);
            Assert.AreEqual(15000.0, line2.money.amount);
            Assert.AreEqual("NOK", line2.money.currency);
            Assert.AreEqual(1, line2.transactions.Count);

            var line3 = msg.lines[2];
            Assert.AreEqual(d20110111, line3.posting_date);
            Assert.AreEqual(6740.40, line3.money.amount);
            Assert.AreEqual("NOK", line3.money.currency);
            Assert.AreEqual(2, line3.transactions.Count);
        }

        [Test]
        public void ValidCremulLongFile()
        {
            var parser = new CremulParser();
            parser.parse(ResolveFilePath("CREMUL0003.txt"));

            var d20130411 = new DateTime(2013, 4, 11);

            var msg = @parser.messages[0];
            Assert.IsTrue(msg.header.msg_id.Contains("CREMUL"));
            Assert.AreEqual(d20130411, msg.header.created_date);

            Assert.AreEqual(4, msg.number_of_lines);

            var line1 = msg.lines[0];
            Assert.AreEqual(d20130411, line1.posting_date);
            Assert.AreEqual(3000.0, line1.money.amount);
            Assert.AreEqual("NOK", line1.money.currency);

            Assert.AreEqual(12, line1.transactions.Count);

            var tx = line1.transactions[0];
            Assert.AreEqual(new DateTime(2013, 4, 10), tx.posting_date);
            Assert.AreEqual(250.0, tx.money.amount);
            Assert.AreEqual("NOK", tx.money.currency);
            Assert.AreEqual("12345678901", tx.payer_account_number);
            Assert.AreEqual("20132065978", tx.invoice_ref);
            Assert.AreEqual(RefTypes.KID, tx.invoice_ref_type);
        }

        [Test]
        public void ValidCremulAddresParse()
        {
            var parser = new CremulParser();
            parser.parse(ResolveFilePath("CREMUL0002-27.05.14.DAT"));

            var msg = @parser.messages[0];

            var tx = msg.lines[0].transactions[0];
            var txPayer = tx.payer_nad;
            Assert.AreEqual("PL",txPayer.type);
            Assert.AreEqual("Ole Thomessen", txPayer.nad_lines[0]);
            Assert.AreEqual("St. Nikolas-Gate 7", txPayer.nad_lines[1]);
            Assert.AreEqual("1706 SARPSBORG", txPayer.nad_lines[3]);
        }

        [Test]
        public void ValidCremulWithEmptyFIIElems()
        {
            var parser = new CremulParser();
            parser.parse(ResolveFilePath("CREMUL0001-27.05.14.DAT"));

            var msg = parser.messages[0];
             Assert.IsTrue(msg.header.msg_id.Contains("CREMUL"));

             Assert.IsNull(msg.lines[0].transactions[0].payer_account_number);     
        }

        [Test, Ignore]
        public void ValidCremulMultiMessageFile()
        {

        }
    }
}
