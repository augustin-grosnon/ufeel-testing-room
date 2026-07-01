import cv2
import torch
from PIL import Image
import logging

from client_base import ClientBase

from emotion_detection import (
    EmotionModel,
    load_checkpoint,
    InferencePipeline,
    MTCNNCropTransform,
    HeadCalibration,
    load_config
)

logging.basicConfig(
    filename="client_base.log",
    filemode="a",
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG,
)


class EmotionDetector(ClientBase):
    def __init__(self, model_path, config_path, device="cpu"):
        super().__init__("127.0.0.1", 4100)

        self.handlers = {
            "emotion_detection": self.toggle_emotion_detection,
        }

        self.device = torch.device(device)

        config = load_config(config_path)

        model_cfg = config["model"]
        data_cfg = config["data"]

        self.model = EmotionModel(backbone_name=model_cfg["backbone_name"])
        self.model = load_checkpoint(self.model, model_path, device=self.device)

        face_crop = MTCNNCropTransform(
            image_detection_size=data_cfg["img_size"]
        )

        self.pipeline = InferencePipeline(
            model=self.model,
            device=self.device,
            face_crop=face_crop,
            img_size=data_cfg["img_size"],
        )

        self.pipeline.set_calibration(
            HeadCalibration(thresholds=model_cfg["thresholds"])
        )

        self.process_enable = False
        self.selected = []

    def toggle_emotion_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        logging.info(f"Emotion detection {status} {state}")

    def process(self, frame, counter, show_window=True):
        if not self.process_enable:
            return

        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        img = Image.fromarray(rgb)

        out = self.pipeline.predict(img)

        if "binary" not in out:
            return

        selected = [k for k, v in out["binary"].items() if v]

        if counter == 0:
            self.selected = selected
            self.send(out["heads"])

        selected = self.selected

        if show_window:
            self._draw(frame, out["heads"], selected)

    def _draw(self, frame, probs, selected):
        y = 30

        for k, v in probs.items():
            cv2.putText(
                frame,
                f"{k}: {v:.2f}",
                (10, y),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.7,
                (255, 255, 255),
                2,
            )
            y += 25

        y = 30

        for i, e in enumerate(selected):
            cv2.putText(
                frame,
                e,
                (300, y + i * 25),
                cv2.FONT_HERSHEY_SIMPLEX,
                0.7,
                (0, 0, 0),
                2,
            )
