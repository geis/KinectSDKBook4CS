using System;
using System.Collections.Generic;
using System.Text;
using Hisui.Testing;

namespace Hisui.Std
{
  class StorageTest : Test<Core.IStorage>
  {
    [Check]
    void CheckCount()
    {
      Assert.That( Target.Count >= 0 );
    }

    [Check]
    void CheckContains()
    {
      foreach ( int id in Target.IDs ) Assert.That( Target.Contains( id ) );
    }

    [Check]
    void CheckIDs()
    {
      int count = 0;
      foreach ( int id in Target.IDs ) ++count;
      Assert.Equal( Target.Count, count );
    }
  }

  [Testee]
  class Int32StorageTest : Test<Core.IStorage<int>>
  {
    [Testee]
    static object CreateInt32Storage()
    {
      return new Core.Storage<int>();
    }

    [Test]
    void TestPut()
    {
      int count = Target.Count;
      int id = Target.Put( 10 );
      Assert.Equal( Target.Count, count + 1 );
      Assert.That( Target.Contains( id ) );
      Assert.Equal( Target[id], 10 );
    }
  }
}
