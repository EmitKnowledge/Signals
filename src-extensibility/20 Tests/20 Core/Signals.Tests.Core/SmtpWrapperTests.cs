using System;
using Signals.Core.Common.Smtp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Signals.Aspects.Configuration.File;
using Signals.Aspects.DI;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.Core
{
    public class SmtpWrapperTests
    {
        private static BaseTestConfiguration _configuration = BaseTestConfiguration.Instance;
        
        private string rootEmail = _configuration.SmtpConfiguration.RootEmail;
        private string rootDomain = _configuration.SmtpConfiguration.RootDomain;

        private string _subject => "Subject";
        private string _body => "Body";
        private string _from => $"{rootEmail}@{rootDomain}";

        private string _to => $"{rootEmail}@{rootDomain}";
        private string _cc => $"{rootEmail}+1@{rootDomain}";
        private string _bcc => $"{rootEmail}+2@{rootDomain}";
        
        private SmtpClientWrapper _smtpWrapper = new SmtpClientWrapper()
        {
            Server = _configuration.SmtpConfiguration.Server,
            Port = _configuration.SmtpConfiguration.Port,
            Username = _configuration.SmtpConfiguration.Username,
            Password =_configuration.SmtpConfiguration.Password
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
