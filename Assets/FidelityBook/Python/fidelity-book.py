import os
import time
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader
from torchvision import datasets, transforms

# 1. SETUP CONSTANTS & CONFIGURATION
IMAGE_HEIGHT = 32
IMAGE_WIDTH = 64
CHANNELS = 1
BATCH_SIZE = 32
NUM_CLASSES = 9
NUM_EPOCHS = 100 #0 #500 #1000  # Epoch variable
DATA_DIR = "image_data"  # Root directory containing 9 subfolders (one per class)
ONNX_FILE_PATH = "fidelity-book.onnx"

# 1. Define the Preprocessing Pipeline
# Converts 3-channel images to 1-channel grayscale and resizes to 32x64 (HxW)
transform = transforms.Compose([
    transforms.Grayscale(num_output_channels=1), 
    transforms.Resize((IMAGE_HEIGHT, IMAGE_WIDTH)),                 # Height=32, Width=64
    transforms.ToTensor(),
    transforms.Normalize((0.5,), (0.5,))
])

# 2. Load Images from Folder
# Expects a directory structure like: data/train/class_1/*.png
# Replace 'data/train' with your actual folder path

if not os.path.exists(DATA_DIR):
    print(f"Creating dummy directory '{DATA_DIR}'. Please place your class folders inside.")
    os.makedirs(DATA_DIR, exist_ok=True)
    # Creating a fake structure for script to run without crashing initially
    os.makedirs(os.path.join(DATA_DIR, "dummy_class"), exist_ok=True)

dataset = datasets.ImageFolder(root=DATA_DIR, transform=transform)
dataloader = DataLoader(dataset, batch_size=BATCH_SIZE, shuffle=True)

# Determine number of classes dynamically
num_classes = len(dataset.classes) if len(dataset.classes) > 0 else 10

# 3. Define a Simple Model compatible with 1x32x64 Input
class ConvNet(nn.Module):
    def __init__(self, num_classes):
        super(ConvNet, self).__init__()
        self.features = nn.Sequential(
            nn.Conv2d(CHANNELS, 16, kernel_size=3, padding=1), # 1 input channel (grayscale)
            nn.ReLU(),
            nn.MaxPool2d(2, 2),                         # Output: 16 x 16 x 32
            nn.Conv2d(16, 32, kernel_size=3, padding=1),
            nn.ReLU(),
            nn.MaxPool2d(2, 2)                          # Output: 32 x 8 x 16
        )
        self.classifier = nn.Linear(32 * 8 * 16, num_classes)

    def forward(self, x):
        x = self.features(x)
        x = torch.flatten(x, 1)
        x = self.classifier(x)
        return x

# Initialize system
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model = ConvNet(num_classes=num_classes).to(device)
criterion = nn.CrossEntropyLoss()
optimizer = optim.Adam(model.parameters(), lr=0.001)

# 4. Training Loop showing Epoch Progress
print("Starting Training...")
start_time = time.perf_counter()

for epoch in range(NUM_EPOCHS):
    model.train()
    running_loss = 0.0
    correct = 0
    total = 0
    
    for images, labels in dataloader:
        images, labels = images.to(device), labels.to(device)
        
        # Forward pass
        outputs = model(images)
        loss = criterion(outputs, labels)
        
        # Backward pass and optimize
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        
        running_loss += loss.item() * images.size(0)
        _, predicted = outputs.max(1)
        total += labels.size(0)
        correct += predicted.eq(labels).sum().item()
        
    epoch_loss = running_loss / len(dataloader.dataset)
    epoch_acc = 100. * correct / total

    n = int(epoch_loss * 150)
    if n > 60:
        n = 60
    graph = '|' * n    

    print(f"Epoch [{epoch+1}/{NUM_EPOCHS}] - Loss: {epoch_loss:.4f} - Acc: {epoch_acc:.2f}% \t{graph}")

print("Training Finished.")
input_size = IMAGE_HEIGHT * IMAGE_WIDTH * CHANNELS
total_params = torch.nn.utils.parameters_to_vector(model.parameters()).numel()
end_time = time.perf_counter()
execution_time = int(end_time - start_time)

print("Training Complete...\n")
print("Epochs:\t\t", NUM_EPOCHS)
print(f"Loss:\t\t {epoch_loss:.3f}")
print("Image Files:\t", len(dataset))
print("Image (H x W):\t", IMAGE_HEIGHT, "x", IMAGE_WIDTH)
print("Input Size:\t", input_size)
print("Output Size:\t", NUM_CLASSES)
print(f"Parameters:\t {total_params:,}")

minutes, remainder = divmod(execution_time, 60)
print(f"Execution time:\t {minutes:02d}:{remainder:02d} (mm:ss)\n")


# 5. Export Trained Model to ONNX format
model.eval()
# Create dummy input tracking the shape: Batch size 1, 1 Channel, 32 Height, 64 Width
dummy_input = torch.randn(1, CHANNELS, IMAGE_HEIGHT, IMAGE_WIDTH, device=device)

torch.onnx.export(
    model, 
    dummy_input, 
    ONNX_FILE_PATH, 
    export_params=True, 
    opset_version=18, 
    do_constant_folding=True, 
    input_names=['input'], 
    output_names=['output'],
    dynamic_axes={'input': {0: 'batch_size'}, 'output': {0: 'batch_size'}} # Allows variable batch sizes during inference
)

print(f"Model successfully converted and saved to {ONNX_FILE_PATH}")
