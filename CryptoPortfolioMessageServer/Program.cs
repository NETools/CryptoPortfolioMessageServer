// BISMILLAH

using CryptoPortfolioMessageServer.Receiver;
using System.Security.Cryptography;

Console.WriteLine("-1-");

var privateKey = "<RSAKeyValue><Modulus>tqfPpisNfHYJe3v2fBdMyvVtJWnimdK1rq+g3uKgNlYHFIfCIeLJ/gFcD8bcTRCgI8gSEzu48sGgnxzzSh/Gj7BSVrq2dTlFC5ma3z+t7khP5NYTT2JmlRgBi3plMM4rdqi8p47QWvzMojuut3wXsS+9XDnJ+0iVhw4XLcTs6kl28Y5z6z/GOzhC8W9XgPoJLWSr9kgNtTPIHzfIz9eaTvqA0np7iht6pzQxqJuhQKX7cGV3WztpijvT/KYdJrNXq+aAmra11I6i6rpHDJ9O2Sor7IFu2o/3vcsNTyxaYwCNvpCHNoQwqvSHgT91Io8xZdm/UJGMPeDAWEDHRYtSaguhe+43A/pCOaNzEtbyoEIUe+igq2iFUMi4ReEJvouv3piSlt16rjUgebic7+lHPuQDv3omIcM0mVHErceohfDxTAeTTAdL/S7tUOZ0rd9dm3jbFBE6rVDl79orjs6hKj8pQTWIv/BF51pFmIesNy0/Xc1rBXuH4ocF1Kzgkk7mZY5WrBkGoS77/uS/u8sMkVPCemUmS8szahIF0pcnb1hYPdVVuYeni9vL0eknUm1P30pWFaN9IPA4qzskCeu8I16RJO3q+u43wCxLxwbBvEhwxgKW6iZ68gnV2R/6LG8F1Z52n2taCFBKBEUITtnJQwocMCn2WJwKQKDUqCbJuPk=</Modulus><Exponent>AQAB</Exponent><P>1JwtuBrAZlWor1UOTnMe+Y2LIzcINu0YKk2bnx/xflK1bZ7/j3SvDNunjTqSj6eyt/2BVT3xAZ82MOItMi4uqwmH2xcMoZcr5JkTH5n6w5K5U1hXi743aW1bnAOfjTXZaj2NOEm6dsUtrDBb1LNIidSm4NbTmDEfIlqA1VoC1Pusmp3g2sv9MX3Y9DFXx7k387GvhrtQ4f6kgUHReJf1D3nubjBUFyipymCdqe69Svm/d4vCWSFcVItmASPg4+3H5lJG2cg9iScHlDN0K+9KXdseCNaS0B+2ByPQZiGRbO2HopRlIFPdsAhvW6+vCVZLyhFg3LbO+6aI0YHJMKTT5w==</P><Q>2+6nxwSMz4g9oaGWMWmS2eVHVdhWR5ddWMIYDLeb3WDH0y0XLzhTd+FV0Me3k2NqHguRUlm3Brj60ybeuH+LsZT1+Ya/FNfIkqf15ElhV3/pY4n99+8SlDcwgHwwOo0iiLNOrbAcTOxgYyJxPCUoKqeQG5Prs4iHoeozOC9urEjL1DKC8bSzbdrJyJiI9+zrvkwFYjB8DbRVYd353UpvjPlYweoQF/cHDIJWSD/acfDcRMAQ1l/LlhWVaBqHTHPJgrjrF/BPDZBpyraC2EFIL2wk66xIVEcekIIuoUvLq2SVNQaqXPtFAyvPzcHg1I9874pUSalEF+3a4N3EVQlwHw==</Q><DP>Qxd50fQXLPPxBEs59YWci7Gf65/tJrARBSBycHd7FJrzB8G58cInj04thIKsR0rrOeZ0jrq6OAHO7wQgsQojzfN2/Ks8YC29OykXAMztdhPyCZwCHGYNRGavFv+JtNz+W+lKjC2v+tTkQccM44Uj6eLCOodwREGUCpo2t5zs8drtr+kMUN84agrFkOGVSsBnKK60rjakInzh2qwzdvO08IpQtFLp5lj9o5BzkHF3KJbBVe8ziD7fzQokSU3SJPsfLs5d71GiIYrAeIL4MPkVJaMEylM0BxtKrNwfYmtLYe1e9O4RL29nsKoowe0htjeWORDc2SmTDZSS7lIfdAs/lw==</DP><DQ>JTWrjTBn4nTaeI4uKw4BmnwAJbYEYvhpAqtUBlLKPBoztLbcRVDyTkYBjuE7XeRj/9FVNOmY3bpONYtOA0kRCWp6c00o5w3bEXT5VTZVEcjCY+Tk4x2/tvPfcN2SC1X1kctEdH5nS1xyslMRdWuT8COJbiEIr8dZqztEjyz3PI7UbYJeRSaddOifWyDzi366I+gYwRQS6P6ps3Uq4RXG6cMGEUjYkDVrK+KhVnf3LW/i1H7qWlq6xxm/vW0/lt/AKC3BGLu/pDvaqkFWwd/JprxVQspTMm8V7L0ZxpBcVRMfa2lFSZX5hx/SeK1krJHaQWzV9eB9dM75STI8FWcTYQ==</DQ><InverseQ>IWIqTexSwqHLfSQYMj/pXKcV3aAYXu6+0LjBYYgm6v/HMrP2iobqBrPd2ZHJY8xBQPRyJnMtLhmhRuxXHCDrU0NFvEEVyjydnMFrPhvy1hM944asJgFQ34GNw/T8QvmraHpP4IBNOPoxMTbXkImNCJtt808jU2KEC6/EVzWpjZsthfHf0QlmYgfOwxB7yuZNbmWhXzTfgKum3GVMhTdBWq1cvE/hL4z2X0z0Dtt6xBugAzTLzpViF41mXlyUfphMDA8fVicigNxe0cOEWKTUoawJIrLUiuXgZ7vPCaYBbqfGadJEbH3EmCe2zBd8jZq7uzZ2KdjEQQ94ESVN4zWACA==</InverseQ><D>CHVvBVf7EOd6evB7srV1hfSlsFTd/JeoeEWD4q6GEt4bB9VdpSoWhp/TfpuL+jmp4N5UD/X1E7Dp2lSB483i6SaWOZP9/SHA08+yP2J4kI9iu8s6K7P34jGxaJ8rbn4tvxoDWpMfjQcmBuFSrwadJkR6tXmro+pxmkaJxkHXYuYyDu+06JEUgguR9hJc8AkkQajm2cnucJTMUL3Jm+51I/Ev5A9uBzBMls+w+Qj0B8VoBoV8aIow0gkBQTMEKJWnmrta+084znYoj0w/HBOImD/+tr4rvw1pwK1VmXQJX4pYBho1WM34XwJiABdLOmV9OSVDWdzTrM3qumuNjufr0g9d1ilQ5NLlfoJUZtu7lUYkUG9tQC2aCo2IR/UUtPhu7Kce9zH1/lx1zv13KfQrNryE/lnl5TdFG8N1zAIqzyC4UPpFKkMUaWHwJYgViL7ZZfm9pj1x7caeQZs4B+mugmBfQDnV+XBqElSJN+jx18Ah8Xs+UwHFwDi9ruASLd+3/v41vsVkcoaEXzEujo1RmgQi0650GwnZQdi6KF3ab6vR56YxQDuQVxE8I4Nbc3HVzrIWQYuTgK3En4zN4JaHlDjVkjIht8BvnbafL/3ecRtMydfuD9/dxJtUIfp/Et8GBamX42t3r7h9mJo1efS6TSDLQmchH2k5ZBja7XfGiL0=</D></RSAKeyValue>";

if(args.Length > 0)
{
	var arg0 = args[0];

	if (arg0 == "-GenKeys")
	{
		KeyGen();
		return;
	}
	else if (arg0 == "-SetKey")
	{
		var privateKeyXmlPath = args[1];
		privateKey = File.ReadAllText(privateKeyXmlPath);
	}
}




var receiver = new MessageReceiver("178.25.225.236:8000", privateKey);
await receiver.Start();

 
Console.ReadLine();



static void KeyGen()
{
	using var rsa = new RSACryptoServiceProvider(4096);

	string publicKey = rsa.ToXmlString(false);
	string privateKey = rsa.ToXmlString(true);

	Console.WriteLine("###########################################################");
	Console.WriteLine(publicKey);
	Console.WriteLine("###########################################################");

	Console.WriteLine("###########################################################");
	Console.WriteLine(privateKey);
	Console.WriteLine("###########################################################");
}