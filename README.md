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
- Branching using ? 0 ms        
  int a = i > na_i [1] ? i : na_i [1] ;
- No branching 0 ms            
  int a = na_i [1] ^ ((i ^ na_i [1]) & -(i << na_i [1])) ;
- No branching with if 0 ms    
  int a = i ;if ( i <= na_i [1] ) a = na_i [1] ;
- Branching if else 0 ms       
  if ( i > na_i [1] ) { a = i ; } else { a = na_i [1] ; }



Iterating 10 million of elements.

Main thread no burst
- Branching using ? 49 ms        
  z = i > b ? i : b ;
- No branching 28 ms             
  z = b ^ ((i ^ b) & -(i << b)) ;
- No branching with if 63 ms     
  z = i ; if ( i <= b ) z = b ;
- Branching if else 78 ms        
  if ( a > b ) { z = a ; } else { z = b ; }

Parallel For With burst         (equivalent)
- Branching using ? ~4.1 ms        
  int a = i > na_i [1] ? i : na_i [1] ;
- No branching ~4.5 ms            
  int a = na_i [1] ^ ((i ^ na_i [1]) & -(i << na_i [1])) ;
- No branching with if ~4.5 ms    
  int a = i ;if ( i <= na_i [1] ) a = na_i [1] ;
- Branching if else ~4.5 ms       
  if ( i > na_i [1] ) { a = i ; } else { a = na_i [1] ; }
  
