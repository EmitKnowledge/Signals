using Signals.Core.Common.Smtp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Xunit;

namespace Signals.Tests.Core
{
    public class SmtpWrapperTests
    {
        private string rootEmail = "vojdan.gicharovski";
        private string rootDomain = "gmail.com";

        private string _subject => "Subject";
        private string _body => "Body";
        private string _from => $"{rootEmail}@{rootDomain}";


        private string _to => $"{rootEmail}@{rootDomain}";
        private string _cc => $"{rootEmail}+1@{rootDomain}";
        private string _bcc => $"{rootEmail}+2@{rootDomain}";

        private SmtpClientWrapper _smtpWrapper = new SmtpClientWrapper()
        {
			Server = "smtp.gmail.com",
            Port = 587,
            Username = string.Empty,
            Password = string.Empty
		};

        [Fact]
        public void SendEmail_GetsSent()
        {
            var message = new MailMessage(_from, _to, _subject, _body);

            _smtpWrapper.Send(_from, _to, _subject, _body);
            _smtpWrapper.Send(message);

            _smtpWrapper.SendMailAsync(_from, _to, _subject, _body).GetAwaiter().GetResult();
            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendWhitelistedEmail_GetsSent()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { _to };

            var message = new MailMessage(_from, _to, _subject, _body);

            _smtpWrapper.Send(_from, _to, _subject, _body);
            _smtpWrapper.Send(message);

            _smtpWrapper.SendMailAsync(_from, _to, _subject, _body).GetAwaiter().GetResult();
            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendUnwhitelistedEmail_GetsBlocked()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { "no.whitelist@gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);

            _smtpWrapper.Send(_from, _to, _subject, _body);
            _smtpWrapper.Send(message);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(_from, _to, _subject, _body).GetAwaiter().GetResult();
            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendWhitelistedDomainEmail_GetsSent()
        {
            _smtpWrapper.WhitelistedEmailDomains = new List<string> { _to.Split('@').Last() };

            var message = new MailMessage(_from, _to, _subject, _body);

            _smtpWrapper.Send(_from, _to, _subject, _body);
            _smtpWrapper.Send(message);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(_from, _to, _subject, _body).GetAwaiter().GetResult();
            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendUnwhitelistedDomainEmail_GetsBlocked()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { "no.whitelist.gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);

            _smtpWrapper.Send(_from, _to, _subject, _body);
            _smtpWrapper.Send(message);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(_from, _to, _subject, _body).GetAwaiter().GetResult();
            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }


        [Fact]
        public void SendWhitelistedCCEmail_GetsSent()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { _to, _cc };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.CC.Add(_cc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendUnwhitelistedCCEmail_GetsBlocked()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { "no.whitelist@gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.CC.Add(_cc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendWhitelistedDomainCCEmail_GetsSent()
        {
            _smtpWrapper.WhitelistedEmailDomains = new List<string> { "gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.CC.Add(_cc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendUnwhitelistedDomainCCEmail_GetsBlocked()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { "no.whitelist.gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.CC.Add(_cc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }


        [Fact]
        public void SendWhitelistedBCCEmail_GetsSent()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { _to, _bcc };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.Bcc.Add(_bcc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendUnwhitelistedBCCEmail_GetsBlocked()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { "no.whitelist@gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.Bcc.Add(_bcc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendWhitelistedDomainBCCEmail_GetsSent()
        {
            _smtpWrapper.WhitelistedEmailDomains = new List<string> { "gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.Bcc.Add(_bcc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendUnwhitelistedDomainBCCEmail_GetsBlocked()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { "no.whitelist.gmail.com" };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.Bcc.Add(_bcc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }

        [Fact]
        public void SendWhitelistedCCAndBCCEmail_GetsBlocked_NoTo()
        {
            _smtpWrapper.WhitelistedEmails = new List<string> { _cc, _bcc };

            var message = new MailMessage(_from, _to, _subject, _body);
            message.CC.Add(_cc);
            message.Bcc.Add(_bcc);

            _smtpWrapper.Send(message);
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);

            _smtpWrapper.SendMailAsync(message).GetAwaiter().GetResult();
            Assert.Contains(message.CC.ToList(), x => x.Address == _cc);
            Assert.Contains(message.Bcc.ToList(), x => x.Address == _bcc);
            Assert.Contains(message.To.ToList(), x => x.Address == _to);
        }
    }
}
