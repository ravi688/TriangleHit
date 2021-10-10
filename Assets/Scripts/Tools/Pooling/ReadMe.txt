

Pooling System
------------------------------------------
Pooling System is the core of Shooting games because if you want to create highly realistic ballistics then the Pooling System will 
determine the actual performance of your game.
This is very useful when you want to work with hundreds of entities but you know the maximum number of entities that can be in active state.
This Pooling System is designed to be flexible and high performance.


There are follwing types of Pooling Systems in this project: 
1. RandomAccessPool
	a. StaticRandomAccessPool
	b. DynamicRandomAccessPool
2. ContiguousPool
	a. StaticContiguousPool
	b. DynamicContiguousPool

Static Pools store their data in static arrays which are faster if you access the data sequential because cache hit ratio will be very high.
Dynamic Pools are implementation defined but are also good to use.