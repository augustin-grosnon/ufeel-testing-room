import torch
import torch.nn as nn
import torch.nn.functional as F

class GiMeFive(nn.Module):
    def __init__(self):
        super().__init__()
        self.conv1, self.bn1 = nn.Conv2d(3, 64, 3, padding=1), nn.BatchNorm2d(64)
        self.conv2, self.bn2 = nn.Conv2d(64, 128, 3, padding=1), nn.BatchNorm2d(128)
        self.conv3, self.bn3 = nn.Conv2d(128, 256, 3, padding=1), nn.BatchNorm2d(256)
        self.conv4, self.bn4 = nn.Conv2d(256, 512, 3, padding=1), nn.BatchNorm2d(512)
        self.conv5, self.bn5 = nn.Conv2d(512, 1024, 3, padding=1), nn.BatchNorm2d(1024)
        self.pool = nn.AdaptiveAvgPool2d((1,1))
        self.fc1, self.fc2 = nn.Linear(1024, 2048), nn.Linear(2048, 1024)
        self.dropout1, self.dropout2 = nn.Dropout(0.2), nn.Dropout(0.5)
        self.fc3 = nn.Linear(1024, 6)

    def forward(self, x):
        for conv, bn in [(self.conv1, self.bn1), (self.conv2, self.bn2),
                            (self.conv3, self.bn3), (self.conv4, self.bn4)]:
            x = self.dropout1(F.max_pool2d(F.relu(bn(conv(x))), 2))
        x = F.max_pool2d(F.relu(self.bn5(self.conv5(x))), 2)
        x = self.pool(x).view(x.size(0), -1)
        x = self.dropout2(F.relu(self.fc1(x)))
        return self.fc3(F.relu(self.fc2(x)))

device = torch.device("cpu")
model_path = "../models/best_GiMeFive.pth"
onnx_path = "../models/best_GiMeFive.onnx"

model = GiMeFive().to(device)
model.load_state_dict(torch.load(model_path, map_location=device))
model.eval()

dummy_input = torch.randn(1, 3, 64, 64, device=device)

torch.onnx.export(
    model,
    dummy_input,
    onnx_path,
    export_params=True,
    opset_version=17,
    do_constant_folding=True,
    input_names=['input'],
    output_names=['output'],
    dynamic_axes={'input': {0: 'batch_size'}, 'output': {0: 'batch_size'}}
)

print(f"ONNX model exported to {onnx_path}")
