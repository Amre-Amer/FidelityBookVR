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
NUM_EPOCHS = 10 #0 #500 #1000  # Epoch variable
DATA_DIR = "image_data"  # Root directory containing 9 subfolders (one per class)
ONNX_FILE_PATH = "fidelity-book.onnx"

# 2. IMAGE LOADING & PREPROCESSING FROM DIRECTORY
# Expected directory structure:
# dataset/
#   ├── class_1/
#   │     └── img1.jpg
#   └── ...
#   └── class_9/
#         └── imgN.jpg

image_transforms = transforms.Compose([
    transforms.Resize((IMAGE_HEIGHT, IMAGE_WIDTH)),  # Resize to 32x64 (H x W)
    transforms.ToTensor(),                           # Convert to tensor (C x H x W)
    transforms.Normalize(mean=[0.5, 0.5, 0.5],       # Normalize pixel values
                         std=[0.5, 0.5, 0.5])
])

# Load dataset using ImageFolder
dataset = datasets.ImageFolder(root=DATA_DIR, transform=image_transforms)
dataloader = DataLoader(dataset, batch_size=BATCH_SIZE, shuffle=True, drop_last=True)

print(f"Loaded {len(dataset)} images across {len(dataset.classes)} classes.")

# 3. DEFINE A SIMPLE CNN MODEL
class SimpleCNN(nn.Module):
    def __init__(self, num_classes):
        super(SimpleCNN, self).__init__()
        self.features = nn.Sequential(
            nn.Conv2d(3, 16, kernel_size=3, padding=1),
            nn.ReLU(),
            nn.MaxPool2d(2, 2),  # Output: 16 x 16 x 32
            nn.Conv2d(16, 32, kernel_size=3, padding=1),
            nn.ReLU(),
            nn.MaxPool2d(2, 2)   # Output: 32 x 8 x 16
        )
        self.classifier = nn.Sequential(
            nn.Flatten(),
            nn.Linear(32 * 8 * 16, 128),
            nn.ReLU(),
            nn.Linear(128, num_classes)
        )

    def forward(self, x):
        x = self.features(x)
        x = self.classifier(x)
        return x

# Initialize model, loss, and optimizer
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model = SimpleCNN(num_classes=NUM_CLASSES).to(device)
criterion = nn.CrossEntropyLoss()
optimizer = optim.Adam(model.parameters(), lr=0.001)

# 4. TRAINING LOOP SHOWING THE EPOCH VARIABLE
print("\n--- Starting Training ---")
start_time = time.perf_counter()
for epoch in range(1, NUM_EPOCHS + 1):
    model.train()
    running_loss = 0.0
    
    for images, labels in dataloader:
        images, labels = images.to(device), labels.to(device)
        
        optimizer.zero_grad()
        outputs = model(images)
        loss = criterion(outputs, labels)
        loss.backward()
        optimizer.step()
        
        running_loss += loss.item() * images.size(0)
        
    epoch_loss = running_loss / len(dataloader.dataset)

    n = int(epoch_loss * 150)
    if n > 80:
        n = 80
    graph = '|' * n    
    print(f"Epoch [{epoch}/{NUM_EPOCHS}] - Loss: {epoch_loss:.4f} {graph}")

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

# 5. EXPORT THE MODEL TO ONNX FORMAT
model.eval()  # Set model to evaluation mode before exporting

# Create dummy input matching the shape: (Batch Size, Channels, Height, Width)
# PyTorch expects (Channels, Height, Width), so it is 3 x 32 x 64
dummy_input = torch.randn(CHANNELS, IMAGE_HEIGHT, IMAGE_WIDTH, device=device)

print(f"Exporting model to {ONNX_FILE_PATH}...")

# torch.onnx.export(
#     model,                      # Model being run
#     dummy_input,                # Model input (or a tuple for multiple inputs)
#     ONNX_FILE_PATH,             # Where to save the model
#     export_params=True,         # Store the trained parameter weights inside the model file
#     opset_version=18,           # The ONNX version to export the model to  11
#     do_constant_folding=True,   # Whether to execute constant folding for optimization
#     input_names=['input'],      # The model's input names
#     output_names=['output'],    # The model's output names
#     dynamic_axes={              # Allow variable batch sizes during deployment
#         'input': {0: 'batch_size'},
#         'output': {0: 'batch_size'}
#     }
# )
# print("ONNX export successful.")

# model = MyModel().eval()
# example_inputs = (torch.randn(1, 10),)

# Simply provide your inputs and shape constraints 
onnx_program = torch.onnx.export(
    model=model,
    args=dummy_input,
    dynamic_shapes={"x": {0: "batch_size"}},
    dynamo=True  # Under the hood, this creates a torch.export.ExportedProgram
)

onnx_program.save(ONNX_FILE_PATH)
