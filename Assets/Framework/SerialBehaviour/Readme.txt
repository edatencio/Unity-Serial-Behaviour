###-
# Author = Edward Atencio
# Name = SerialBehaviour
# Version = 2.3
###~

# Set the "Api Compatibility Level" to ".NET 2.0" in: 
Edit -> Project Settings -> Player

# Drag and drop the Serial Behaviour prefab into the preload scene

# Make sure the SerialSave's path is:
ProductPath\Serial Configuration.txt

# To receive a serial message you have to inherit from ISerialReceiver and subscribe to the Serial Asset

# To send just use the Send(string) method of the Serial Asset