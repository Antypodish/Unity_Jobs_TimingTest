using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Entities ;
using Unity.Jobs;

// using System.Diagnostics;

namespace Antypodish.ECS.Tests
{

    public class JobsBranchingTimingTestSystem : SystemBase
    {

        System.Diagnostics.Stopwatch stopwatch ;

        protected override void OnCreate ()
        {
            stopwatch = new System.Diagnostics.Stopwatch () ;
        }

        
        protected override void OnUpdate ( )
        {

            int i_len = 10000000 ;
            NativeArray <int> na_i = new NativeArray<int> (3, Allocator.TempJob )  ;
        


            int a = 3; int b = 6 ; int z = 0 ;
 
    Debug.LogWarning ( "Main thread no Burst" ) ;
            
            stopwatch.Restart () ;

            for ( int i = 0; i < i_len; i ++ )
            {
                z = i > b ? i : b ;
                // z = a > b ? a : b ;
            }

            // 5ms
    Debug.Log ( "branching ? " + stopwatch.ElapsedMilliseconds ) ;
        
            stopwatch.Restart () ;

            for ( int i = 0; i < i_len; i ++ )
            {
                z = b ^ ((i ^ b) & -(i << b)) ;
                // z = b ^ ((a ^ b) & -(a >> b)) ;
            }

            // 3ms
    Debug.Log ( "no branching " + stopwatch.ElapsedMilliseconds ) ;
        

            stopwatch.Restart () ;

            for ( int i = 0; i < i_len; i ++ )
            {
                z = i ;
                // z = a ;
                if ( i <= b ) z = b ;
                // if ( a <= b ) z = b ;
            }

            // 6ms
    Debug.Log ( "no branching if " + stopwatch.ElapsedMilliseconds ) ;
        
            stopwatch.Restart () ;

            for ( int i = 0; i < i_len; i ++ )
            {
                if ( a > b ) { z = a ; } else { z = b ; } ;
            }

            // 8ms
    Debug.Log ( "if else " + stopwatch.ElapsedMilliseconds ) ;


    Debug.LogWarning ( "Parallel For Jobs Burst" ) ;

             na_i [0] = 1 ;
             na_i [1] = 2 ;
            // JobHandle dep ;

            stopwatch.Restart () ;

            Dependency = new BranchingQuestion ( )
            {
                na_i = na_i,
            }.Schedule ( i_len, 256, Dependency ) ;
            Dependency.Complete () ;

            // 10 ms
    Debug.Log ( "burst branching ? " + stopwatch.ElapsedMilliseconds ) ;

            stopwatch.Restart () ;

            Dependency = new NoBranching ( )
            {
                na_i = na_i,
            }.Schedule ( i_len, 256, Dependency ) ;
            Dependency.Complete () ;

            // 22 ms
    Debug.Log ( "burst no branching " + stopwatch.ElapsedMilliseconds ) ;
        
            stopwatch.Restart () ;

            Dependency = new NoBranchingIf ( )
            {
                na_i = na_i,
            }.Schedule ( i_len, 256, Dependency ) ;
            Dependency.Complete () ;

            // 10 ms
    Debug.Log ( "burst no branching if " + stopwatch.ElapsedMilliseconds ) ;
        
            stopwatch.Restart () ;

            Dependency = new BranchingIfElse ( )
            {
                na_i = na_i,
            }.Schedule ( i_len, 256, Dependency ) ;
            Dependency.Complete () ;

            // 10 ms
    Debug.Log ( "burst branching if else " + stopwatch.ElapsedMilliseconds ) ;

            stopwatch.Stop () ;

            na_i.Dispose () ;

            Debug.LogError ( "Stop" ) ;

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

    



        [BurstCompile]
        struct BranchingQuestion : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray <int> na_i ;
            public void Execute ( int i )
            {
                // for ( int i = 0; i < 1000000; i ++ )
                //{
                int a = i > na_i [1] ? i : na_i [1] ;
    //                 na_i [2] = i > na_i [1] ? i : na_i [1] ;
                //}
            }
        }
    
        [BurstCompile]
        struct NoBranching : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray <int> na_i ;
            public void Execute ( int i )
            {
                //for ( int i = 0; i < 1000000; i ++ )
                //{

                int a = na_i [1] ^ ((i ^ na_i [1]) & -(i << na_i [1])) ;
                    // na_i [2] = na_i [1] ^ ((i ^ na_i [1]) & -(i >> na_i [1])) ;
                //}
            }
        }

    
        [BurstCompile]
        struct NoBranchingIf : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray <int> na_i ;
            public void Execute ( int i )
            {
                //for ( int i = 0; i < 1000000; i ++ )
                //{
                int a = i ;
                if ( i <= na_i [1] ) a = na_i [1] ;
                    // na_i [2] = i ;
                    // if ( i <= na_i [1] ) na_i [2] = na_i [1] ;
                //}
            }
        }
    

    
        [BurstCompile]
        struct BranchingIfElse : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray <int> na_i ;
            public void Execute ( int i )
            {
                //for ( int i = 0; i < 1000000; i ++ )
                //{
                int a ;
                if ( i > na_i [1] ) { a = i ; } else { a = na_i [1] ; }
                    // if ( i > na_i [1] ) { na_i [2] = i ; } else { na_i [2] = na_i [1] ; }
                //}
            }
        }

    }

}
