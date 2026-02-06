import onnx
model = onnx.load("../models/best_GiMeFive.onnx")
print([o.name for o in model.graph.output])
