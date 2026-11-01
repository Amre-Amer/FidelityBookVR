import os
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader
from torchvision import datasets, transforms

# 1. Define the Preprocessing Pipeline
# Converts 3-channel images to 1-channel grayscale and resizes to 32x64 (HxW)
transform = transforms.Compose([
    transforms.Grayscale(num_output_channels=1), 
    transforms.Resize((32, 64)),                 # Height=32, Width=64
    transforms.ToTensor(),
    transforms.Normalize((0.5,), (0.5,))
])

# 2. Load Images from Folder
# Expects a directory structure like: data/train/class_1/*.png
# Replace 'data/train' with your actual folder path
data_dir = "image_data" 

if not os.path.exists(data_dir):
    print(f"Creating dummy directory '{data_dir}'. Please place your class folders inside.")
    os.makedirs(data_dir, exist_ok=True)
    # Creating a fake structure for script to run without crashing initially
    os.makedirs(os.path.join(data_dir, "dummy_class"), exist_ok=True)

dataset = datasets.ImageFolder(root=data_dir, transform=transform)
dataloader = DataLoader(dataset, batch_size=32, shuffle=True)

# Determine number of classes dynamically
num_classes = len(dataset.classes) if len(dataset.classes) > 0 else 10

# 3. Define a Simple Model compatible with 1x32x64 Input
class ConvNet(nn.Module):
    def __init__(self, num_classes):
        super(ConvNet, self).__init__()
        self.features = nn.Sequential(
            nn.Conv2d(1, 16, kernel_size=3, padding=1), # 1 input channel (grayscale)
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
num_epochs = 100
print("Starting Training...")

for epoch in range(num_epochs):
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
    print(f"Epoch [{epoch+1}/{num_epochs}] - Loss: {epoch_loss:.4f} - Acc: {epoch_acc:.2f}%")

print("Training Finished.")

# 5. Export Trained Model to ONNX format
model.eval()
# Create dummy input tracking the shape: Batch size 1, 1 Channel, 32 Height, 64 Width
dummy_input = torch.randn(1, 1, 32, 64, device=device)
onnx_filename = "fidelity-book.onnx"

torch.onnx.export(
    model, 
    dummy_input, 
    onnx_filename, 
    export_params=True, 
    opset_version=18, 
    do_constant_folding=True, 
    input_names=['input'], 
    output_names=['output'],
    dynamic_axes={'input': {0: 'batch_size'}, 'output': {0: 'batch_size'}} # Allows variable batch sizes during inference
)

print(f"Model successfully converted and saved to {onnx_filename}")
