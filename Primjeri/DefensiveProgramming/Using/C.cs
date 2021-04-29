using System;

namespace Using
{
  class C : IDisposable
  {
		public string Id { get; set; }
		public void Dispose()
		{			
			Console.WriteLine("**  {0} : Dispose **", Id);
		}

		public C(string id)
		{
			Id = id;
			Console.WriteLine("----> {0} : Ctor", Id);
		}
	}
}
