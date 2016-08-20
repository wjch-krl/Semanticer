using System;
using Semanticer;

namespace Tester
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			SemanticProccessor.Init ();
			Console.WriteLine ("Enter text to calculate Semantics:");
			string text;
			do 
			{
				text = Console.ReadLine ();
				var result = SemanticProccessor.Process (text);
				Console.WriteLine (result);
			}
			while (true);
		}
	}
}
