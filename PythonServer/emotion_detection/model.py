import torch.nn as nn
from torchvision import models


class BinaryHead(nn.Module):
    def __init__(self, in_features: int):
        super().__init__()
        self.linear = nn.Linear(in_features, 1)

    def forward(self, x):
        return self.linear(x)


class EmotionModel(nn.Module):
    def __init__(self, backbone_name: str):
        super().__init__()

        if backbone_name == "resnet34":
            backbone = models.resnet34(weights=None)
            in_features = backbone.fc.in_features
            backbone.fc = nn.Identity()
        else:
            raise ValueError(f"Unsupported backbone: {backbone_name}")

        self.backbone = backbone

        self.heads = nn.ModuleDict({
            "angry": BinaryHead(in_features),
            "contemptuous": BinaryHead(in_features),
            "disgusted": BinaryHead(in_features),
            "fearful": BinaryHead(in_features),
            "happy": BinaryHead(in_features),
            "neutral": BinaryHead(in_features),
            "sad": BinaryHead(in_features),
            "surprised": BinaryHead(in_features),
        })

    def forward(self, x):
        features = self.backbone(x)

        return {
            k: head(features).squeeze(1)
            for k, head in self.heads.items()
        }
