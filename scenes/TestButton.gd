extends Button

var _hover = false

func _gui_input(event):
	if event is InputEventMouseButton and event.is_pressed():
		get_tree().set_input_as_handled()
		print("Mouse click on button.")
