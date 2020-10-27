using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Entities ;
using Unity.Burst ;
using Unity.Jobs ;
using System.Linq;

namespace Antypodish.ECS.Tests
{

    public class JobsHashmapTimingTestSystem : SystemBase
    {

        System.Diagnostics.Stopwatch stopwatch ;

        protected override void OnCreate ()
        {
            stopwatch = new System.Diagnostics.Stopwatch () ;
            stopwatch.Restart () ;
            stopwatch.Start () ;

            int i_len = 10000 ;

            NativeMultiHashMap <int,int> dic = new NativeMultiHashMap <int, int> ( i_len, Allocator.Temp ) ;
            NativeMultiHashMap <int,int> dic2 = new NativeMultiHashMap <int, int> ( i_len, Allocator.TempJob ) ;

            NativeHashMap <int, int> dic3 = new NativeHashMap <int, int> ( i_len, Allocator.Temp ) ;
            for ( int i = 0; i < i_len; i ++ )
            {
                var v2 = new Vector2Int ( Random.Range ( 10, -10 ), Random.Range ( -1000, 1000 ) ) ;
                dic.Add ( v2.x, v2.y ) ;
                dic2.Add ( v2.x, v2.y ) ;
                dic3.TryAdd ( v2.x, v2.y ) ;
    // Debug.Log ( "v2: " + v2 ) ;
            }
            
    Debug.Log ( "populate hashmaps " + stopwatch.ElapsedMilliseconds + "ms" ) ;
            stopwatch.Restart () ;
            // NativeMultiHashMap <int,int> dic = nmhm_newBornEntitiesPerTile ;

            var withDuplicates = dic.GetKeyArray ( Allocator.Temp ) ;
            
    Debug.Log ( "multi hashmap get keys " + stopwatch.ElapsedMilliseconds + "ms" ) ;
            stopwatch.Restart () ;

            NativeArray <int> withDuplicates2 = new NativeArray <int> ( dic2.Count (), Allocator.TempJob, NativeArrayOptions.UninitializedMemory ) ;
            
            Dependency = new GetArray ( )
            {
                dic            = dic2,
                withDuplicates = withDuplicates2 
            }.Schedule ( Dependency ) ;
            Dependency.Complete () ;

    Debug.Log ( "multi hashmap get keys job burst B " + stopwatch.ElapsedMilliseconds + "ms" ) ;
            stopwatch.Restart () ;

            var noDuplicates = dic3.GetKeyArray ( Allocator.Temp ) ;
    Debug.Log ( "hashmap get keys " + stopwatch.ElapsedMilliseconds + "ms" ) ;
            

            /*
            for ( int i = 0; i < noDuplicates.Length; i ++ )
            {
                Debug.Log ( "no duplicates: " + noDuplicates [i] ) ;
            }
            */

            
            stopwatch.Restart () ;

        
            withDuplicates.Sort();
    Debug.Log ( "sort A " + stopwatch.ElapsedMilliseconds + "ms" ) ;
            stopwatch.Restart () ;

            int uniqueCount = withDuplicates.Unique () ;
        
    Debug.Log ( "multi hashmap unique A " + stopwatch.ElapsedMilliseconds + "ms" ) ;
    Debug.Log ( "uniqueCount " + uniqueCount ) ;
            stopwatch.Restart () ;
            Dependency = new Sort ( )
            {
                withDuplicates = withDuplicates2
            }.Schedule ( Dependency ) ;
            Dependency.Complete () ;
        
    Debug.Log ( "sort job burst B " + stopwatch.ElapsedMilliseconds + "ms" ) ;
            stopwatch.Restart () ;

            NativeArray <int> na_i = new NativeArray<int> (3, Allocator.TempJob )  ;
            Dependency = new Unique ( )
            {
                i = na_i,
                withDuplicates = withDuplicates2
            }.Schedule ( Dependency ) ;
            Dependency.Complete () ;
            
            uniqueCount = na_i [0] ;
        
    Debug.Log ( "multi hashmap unique job burst B " + stopwatch.ElapsedMilliseconds + "ms" ) ;
    Debug.Log ( "uniqueCount " + uniqueCount ) ;
            stopwatch.Restart () ;
       
            
            Debug.Log ( "uniqueCount hashmap " + noDuplicates.Length ) ;

        /*
            for ( int i = 0; i < i_len; i ++ )
            {
                Debug.Log ( "B: " + withDuplicates [i] ) ;
            }
        */
            withDuplicates2.Dispose () ;
            dic2.Dispose () ;
            withDuplicates.Dispose () ;
            noDuplicates.Dispose () ;
            dic.Dispose () ;

            Debug.LogError ( "Stop" ) ;

        }

        
        
        protected override void OnUpdate ( )
        {

            /*
            stopwatch.Restart () ;
            stopwatch.Start () ;

            int i_len = 10000 ;

            NativeMultiHashMap <int,int> dic = new NativeMultiHashMap <int, int> ( i_len, Allocator.Temp ) ;
            NativeMultiHashMap <int,int> dic2 = new NativeMultiHashMap <int, int> ( i_len, Allocator.Temp ) ;

            for ( int i = 0; i < i_len; i ++ )
            {
                var v2 = new Vector2Int ( Random.Range ( 10, -10 ), Random.Range ( -1000, 1000 ) ) ;
                dic.Add ( v2.x, v2.y ) ;
                dic2.Add ( v2.x, v2.y ) ;
    // Debug.Log ( "v2: " + v2 ) ;
            }
        

    Debug.Log ( "generate " + stopwatch.ElapsedMilliseconds ) ;

            // NativeMultiHashMap <int,int> dic = nmhm_newBornEntitiesPerTile ;

            var withDuplicates = dic.GetKeyArray ( Allocator.TempJob ) ;
            var withDuplicates2 = dic2.GetKeyArray ( Allocator.TempJob ) ;
        
    Debug.Log ( "get keys " + stopwatch.ElapsedMilliseconds ) ;
            
            
            //for ( int i = 0; i < i_len; i ++ )
            //{
            //    Debug.Log ( "A: " + withDuplicates [i] ) ;
            //}
            
            

        
            withDuplicates.Sort();
    Debug.Log ( "sort " + stopwatch.ElapsedMilliseconds ) ;
            int uniqueCount = withDuplicates.Unique () ;
        
    Debug.Log ( "unique " + stopwatch.ElapsedMilliseconds ) ;
            new Sort ( )
            {
                withDuplicates = withDuplicates2
            }.Schedule ().Complete () ;
        
    Debug.Log ( "sort job burst " + stopwatch.ElapsedMilliseconds ) ;
        
            NativeArray <int> na_i = new NativeArray<int> (3, Allocator.TempJob )  ;
            new Unique ( )
            {
                i = na_i,
                withDuplicates = withDuplicates2
            }.Schedule ().Complete () ;
        
            // uniqueCount = withDuplicates2.Unique () ;
        
    Debug.Log ( "unique job burst " + stopwatch.ElapsedMilliseconds ) ;

    Debug.Log ( "uniqueCount " + uniqueCount ) ;
       
        
            //for ( int i = 0; i < i_len; i ++ )
            //{
            //    Debug.Log ( "B: " + withDuplicates [i] ) ;
            //}
        
            withDuplicates2.Dispose () ;
            dic2.Dispose () ;
            withDuplicates.Dispose () ;
            dic.Dispose () ;
            */
        }

        
        [BurstCompile]
        struct GetArray : IJob
        {
            [ReadOnly]
            public NativeMultiHashMap <int, int> dic ;            
            public NativeArray <int> withDuplicates ;
            public void Execute ()
            {
                withDuplicates = dic.GetKeyArray ( Allocator.Temp ) ;
            }
        }

        [BurstCompile]
        struct Sort : IJob
        {
            public NativeArray <int> withDuplicates ;
            public void Execute ()
            {
                withDuplicates.Sort () ;
            }
        }
    
        [BurstCompile]
        struct Unique : IJob
        {
            public NativeArray <int> i ;
            public NativeArray <int> withDuplicates ;
            public void Execute ()
            {
                i [0] = withDuplicates.Unique () ;
            }
        }

    


    }

}
