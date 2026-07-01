from __future__ import annotations
from typing import Optional
import logging
import torch
from PIL import Image
import numpy as np
from facenet_pytorch import MTCNN

logger = logging.getLogger(__name__)

class MTCNNCropTransform:
    def __init__(self, image_detection_size: int, device: Optional[str] = None, margin: float = 0.1):
        self.image_detection_size = image_detection_size
        self.margin = margin
        if device is None:
            device = "cuda" if torch.cuda.is_available() else "cpu"
        self.device = device
        self.mtcnn = MTCNN(
            image_size=160,
            margin=0,
            min_face_size=20,
            keep_all=False,
            post_process=False,
            select_largest=True,
            device=self.device
        )

    def _align_and_crop(self, img: Image.Image, box: list[float], landmarks: np.ndarray) -> Image.Image:
        x1, y1, x2, y2 = box
        w_box = x2 - x1
        h_box = y2 - y1
        x1 = max(int(x1 - self.margin * w_box), 0)
        y1 = max(int(y1 - self.margin * h_box), 0)
        x2 = min(int(x2 + self.margin * w_box), img.width)
        y2 = min(int(y2 + self.margin * h_box), img.height)
        face_crop = img.crop((x1, y1, x2, y2))
        if landmarks is not None:
            left_eye, right_eye = landmarks[0], landmarks[1]
            dx = right_eye[0] - left_eye[0]
            dy = right_eye[1] - left_eye[1]
            angle = np.degrees(np.arctan2(dy, dx))
            face_crop = face_crop.rotate(angle, resample=Image.BILINEAR, center=((x2 - x1)/2, (y2 - y1)/2))
        return face_crop

    def _resize_and_pad(self, img: Image.Image) -> Image.Image:
        w, h = img.size
        scale = self.image_detection_size / max(w, h)
        new_w, new_h = int(w * scale), int(h * scale)
        img_resized = img.resize((new_w, new_h), Image.BILINEAR)
        if new_w == self.image_detection_size and new_h == self.image_detection_size:
            return img_resized
        new_img = Image.new("RGB", (self.image_detection_size, self.image_detection_size))
        paste_x = (self.image_detection_size - new_w) // 2
        paste_y = (self.image_detection_size - new_h) // 2
        new_img.paste(img_resized, (paste_x, paste_y))
        return new_img

    def __call__(self, imgs: list[Image.Image]) -> list[Image.Image]:
        results = []
        try:
            boxes_list, _, landmarks_list = self.mtcnn.detect(imgs, landmarks=True)
            for img, boxes, landmarks in zip(imgs, boxes_list, landmarks_list):
                if boxes is None or len(boxes) == 0:
                    results.append(img.convert("RGB"))
                    continue
                box = boxes[0]
                face_landmarks = landmarks[0] if landmarks is not None else None
                face_crop = self._align_and_crop(img, box, face_landmarks)
                face_final = self._resize_and_pad(face_crop)
                results.append(face_final)
        except Exception as exc:
            logger.warning("MTCNN batch failed (%s)", exc)
            results = [img.convert("RGB") for img in imgs]

        return results
