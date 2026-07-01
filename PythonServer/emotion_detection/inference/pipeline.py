import torch
from PIL import Image

from .transforms import build_inference_transform
from .calibration import HeadCalibration


class InferencePipeline:
    def __init__(self, model, device, face_crop, img_size):
        self.model = model.to(device)
        self.model.eval()
        self.device = device

        self.transform = build_inference_transform(img_size)

        self.face_crop = face_crop

        self.calibration = None

    def preprocess(self, image: Image.Image):
        if self.face_crop is not None:
            image = self.face_crop([image])[0]

        x = self.transform(image)
        return x.unsqueeze(0)

    def set_calibration(self, calibration: HeadCalibration):
        self.calibration = calibration

    @torch.no_grad()
    def predict(self, image: Image.Image, topk: int = 3):
        x = self.preprocess(image).to(self.device)
        logits = self.model(x)

        probs = {}
        logits_out = {}

        for k, v in logits.items():
            probs[k] = torch.sigmoid(v)[0].item()
            logits_out[k] = v[0].cpu().numpy()

        if self.calibration is not None:
            decisions = self.calibration.apply(probs)
            return {
                "heads": probs,
                "binary": decisions
            }

        return {
            "heads": probs
        }
