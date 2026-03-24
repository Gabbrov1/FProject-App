import math
import os

def hello_world():
    print("Hello, World!")
    return "Hello, World!"


class Circle:
    def __init__(self, radius):
        self.radius = radius

    def area(self):
        return math.pi * self.radius ** 2
    
    def circumference(self):
        return 2 * math.pi * self.radius

if __name__ == "__main__":
    hello_world()
    circle = Circle(5)
    print(f"Area of circle with radius {circle.radius}: {circle.area()}")
    print(f"Circumference of circle with radius {circle.radius}: {circle.circumference()}")