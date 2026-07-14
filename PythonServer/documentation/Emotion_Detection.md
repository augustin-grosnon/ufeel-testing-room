# Emotion Detection

For our emotion detection module, we use our own CNN model trained on a balanced subset of the AffectNet dataset, heavily augmented to improve real-world inference.
For our architecture, we first train a standard classification model with a ResNet34 backbone and a classification head. We then extract the backbone and replace the head with multiple individual heads, which can match either single emotions or compounds (e.g. anger, fear and sadness). We then train this new model.

On the testing split of our dataset, this model reached 95.56% accuracy.
It also validates all the requirements of a real-wrold inference model by handling face face pose, lighting and other pertubations appropriately, which is why we use it in our library.
