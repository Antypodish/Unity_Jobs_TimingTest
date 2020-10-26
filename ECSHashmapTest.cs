using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities ;
using Unity.Collections;
using Unity.Jobs;


namespace Antypodish.ECS.Tests
{

    public class ECSHashmapTest : SystemBase
    {

        System.Diagnostics.Stopwatch stopwatch ;

        protected override void OnCreate ()
        {
            stopwatch = new System.Diagnostics.Stopwatch () ;
            stopwatch.Restart () ;
            stopwatch.Start () ;


            NativeMultiHashMap <int,int> dic = new NativeMultiHashMap <int, int> ( 100000, Allocator.Temp ) ;
            NativeMultiHashMap <int,int> dic2 = new NativeMultiHashMap <int, int> ( 100000, Allocator.Temp ) ;

            for ( int i = 0; i < 100000; i ++ )
            {
                var v2 = new Vector2Int ( Random.Range ( 1001, -1000 ), Random.Range ( -1000, 1000 ) ) ;
                dic.Add ( v2.x, v2.y ) ;
                dic2.Add ( v2.x, v2.y ) ;
    // Debug.Log ( "v2: " + v2 ) ;
            }
        

    Debug.Log ( "generate " + stopwatch.ElapsedMilliseconds ) ;

            // NativeMultiHashMap <int,int> dic = nmhm_newBornEntitiesPerTile ;

            var withDuplicates = dic.GetKeyArray ( Allocator.TempJob ) ;
            var withDuplicates2 = dic2.GetKeyArray ( Allocator.TempJob ) ;
        
    Debug.Log ( "get keys " + stopwatch.ElapsedMilliseconds ) ;
            /*
            for ( int i = 0; i < 1000; i ++ )
            {
                Debug.Log ( "A: " + withDuplicates [i] ) ;
            }
            */

        
            withDuplicates.Sort();
    Debug.Log ( "e1 sort A " + stopwatch.ElapsedMilliseconds ) ;
            int uniqueCount = withDuplicates.Unique () ;
        
    Debug.Log ( "e2 unique A " + stopwatch.ElapsedMilliseconds ) ;
            new Sort ( )
            {
                withDuplicates = withDuplicates2
            }.Schedule ().Complete () ;
        
    Debug.Log ( "e1 sort B " + stopwatch.ElapsedMilliseconds ) ;
        
            NativeArray <int> na_i = new NativeArray<int> (3, Allocator.TempJob )  ;
            new Unique ( )
            {
                i = na_i,
                withDuplicates = withDuplicates2
            }.Schedule ().Complete () ;
        
            // uniqueCount = withDuplicates2.Unique () ;
        
    Debug.Log ( "e2 unique B " + stopwatch.ElapsedMilliseconds ) ;

    Debug.Log ( "uniqueCount " + uniqueCount ) ;
       
        
            for ( int i = 0; i < uniqueCount; i ++ )
            {
                Debug.Log ( "B: " + withDuplicates [i] ) ;
            }
        
            withDuplicates2.Dispose () ;
            dic2.Dispose () ;
            withDuplicates.Dispose () ;
            dic.Dispose () ;

        }

        [BurstCompatible]
        struct Sort : IJob
        {
            public NativeArray <int> withDuplicates ;
            public void Execute ()
            {
                withDuplicates.Sort () ;
            }
        }
    
        [BurstCompatible]
        struct Unique : IJob
        {
            public NativeArray <int> i ;
            public NativeArray <int> withDuplicates ;
            public void Execute ()
            {
                i [0] = withDuplicates.Unique () ;
            }
        }

    


        protected override void OnUpdate ( )
        {
        }

    }

}