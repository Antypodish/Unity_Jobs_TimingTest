# Unity_Jobs_BranchingTimingTest
Testing timing of different branching approaches.

Jobs Branching Timing test has following results:

Iterating 1 million of elements.

Main thread no burst
- Branching using ? 2 ms        
  z = i > b ? i : b ;
- No branching 5 ms             
  z = b ^ ((i ^ b) & -(i << b)) ;
- No branching with if 6 ms     
  z = i ; if ( i <= b ) z = b ;
- Branching if else 7 ms        
  if ( a > b ) { z = a ; } else { z = b ; }

Parallel For With burst         (equivalent)
- Branching using ? 9 ms        
  int a = i > na_i [1] ? i : na_i [1] ;
- No branching 22 ms            
  int a = na_i [1] ^ ((i ^ na_i [1]) & -(i << na_i [1])) ;
- No branching with if 10 ms    
  int a = i ;if ( i <= na_i [1] ) a = na_i [1] ;
- Branching if else 10 ms       
  if ( i > na_i [1] ) { a = i ; } else { a = na_i [1] ; }
