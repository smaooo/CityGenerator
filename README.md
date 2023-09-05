To create a more detailed diagram, we combine the notion of Fractions and recursion with Voronoi diagram generation to add more minor details to our Voronoi diagram rather than just increasing the number of points in the original diagram. The idea of recursive Voronoi or the combination of the Voronoi diagram with Fractals has been investigated theoretically [10] [11] [12]. However, we couldn’t find an actual application of this notion in computer graphics and video games. Therefore, we have also implemented the recursive fractals to create semi-infinite details for the generated city, as long as the developed city is not above the hardware limitation.![image](https://github.com/smaooo/CityGenerator/assets/31223968/76afd5c6-5408-49cb-9e47-1443493b23d4)

<img width="213" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/93048082-bdae-443d-873c-f8e625efb58a">
<img width="219" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/87c6eabd-b0f3-4e34-857f-09cbeb2211d4">
<img width="216" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/0b1caea8-aebd-403f-bc62-2bd68f03fc4e">
<img width="214" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/3d839e70-2a87-4ed5-802d-b5b69ebc24fe">
<img width="401" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/9320e953-a7ba-4987-80c5-202f86ff7645">

For adding the recursion to the implemented Voronoi generator, we have separated the generation method for the parent Voronoi diagram and the children because the parent cells will add some limitations to generating random points for the child diagrams.
We started generating points on a plane with a given size for the first Voronoi diagram. However, this is impossible for the child diagrams because each parent cell has a specific form and area. To overcome this problem, we generate a mesh for each parent cell and generate random points in the boundaries of each triangle inside that mesh.
<img width="424" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/933d5bb2-d43f-4290-bffd-a59b44ff33ea">

After generating points for each parent cell, we can create a Voronoi diagram for each. However, we add four extra points outside the generated point cluster for each generation of the Voronoi diagram. Again, for the child diagrams, this will lead us to this problem that the generated diagrams exceed the boundaries of the parent cell. To overcome this problem, we have developed a method that finds the intersection of each edge inside the child diagram with the edges of the parent diagram and trim the edges to intersection points.
<img width="198" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/039a769f-6660-4ce1-8b84-408a010471f9">
<img width="208" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/000ee1f9-b7a2-4189-8fe2-9a3323194fa8">

We have to create meshes for the child cells for the following recursion, but because we have trimmed the edges on the previous iteration, most cells aren’t a polygon. Therefore we have to add edges that close the polygon or, in some cases, that a cell is on the corner of the parent cell add two edges that connect open edges of the polygon to the corner vertex of the parent cell.
<img width="237" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/3ec65ffd-f804-4b96-80d5-9e1ef76280ad">
<img width="213" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/03955544-97f5-467b-bf4a-25ffc8b78d60">

We passed the cell positions and some characteristics to the L-System. After the generation of the Voronoi diagram, for each cell, we calculated the vector from the cell position to each edge mid-point, the rotation angle from the previous vector, and the magnitude of the vector. After passing these structures to the L-System generator, we create a basic L-System production rule (+[…]|+[…]|+[…]….) for only creating branches for the number of cell positions to edge vectors. Then we filled each bracket (branch) inside by the actual production rule. We use the angle between each vector to rotate between branches and calculate the length of each Forward action by dividing the magnitude of the vector by the number of Forward actions inside of each branch. Using this, we can limit the growth of each branch to each edge of the Voronoi cell.
<img width="217" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/36a8f8b6-2787-4de2-a8c3-a1e61bfb07a0">
<img width="216" alt="image" src="https://github.com/smaooo/CityGenerator/assets/31223968/7c0da408-b8cd-45a4-aa34-a2fb1d8079c2">















